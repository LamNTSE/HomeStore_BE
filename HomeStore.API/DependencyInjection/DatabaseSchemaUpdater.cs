using HomeStore.DAL;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HomeStore.API.DependencyInjection;

public class DatabaseSchemaUpdater : IDatabaseSchemaUpdater
{
    private readonly HomeStoreV2Context _context;

    public DatabaseSchemaUpdater(HomeStoreV2Context context)
    {
        _context = context;
    }

    public async Task UpdateSchemaAsync()
    {
        var hasMigrationHistory = await TableExistsAsync("__EFMigrationsHistory");
        var hasUserTables = await HasAnyUserTableAsync();

        if (hasMigrationHistory || !hasUserTables)
        {
            var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
            if (pendingMigrations.Count > 0)
            {
                Console.WriteLine($"[SchemaUpdater] Applying {pendingMigrations.Count} pending migration(s).");
            }
            else
            {
                Console.WriteLine("[SchemaUpdater] No pending migrations.");
            }

            await _context.Database.MigrateAsync();
        }
        else
        {
            // Existing database created outside EF migrations.
            Console.WriteLine("[SchemaUpdater] Existing schema without migration history detected. Applying compatibility patches only.");
        }

        // Keep idempotent compatibility patches for old databases.
        await EnsureNewTablesAsync();
    }

    private async Task EnsureNewTablesAsync()
    {
        // Backfill columns needed by current Order model on old databases.
        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Orders')
               AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'VoucherId')
            BEGIN
                ALTER TABLE [Orders] ADD [VoucherId] INT NULL;
                PRINT '[SchemaUpdater] Added column VoucherId to Orders.';
            END");

        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Orders')
               AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DiscountAmount')
            BEGIN
                ALTER TABLE [Orders] ADD [DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                PRINT '[SchemaUpdater] Added column DiscountAmount to Orders.';
            END");

        // Vouchers table
        await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Vouchers')
            BEGIN
                CREATE TABLE [Vouchers] (
                    [VoucherId]      INT IDENTITY(1,1) PRIMARY KEY,
                    [Code]           NVARCHAR(50)    NOT NULL,
                    [DiscountType]   NVARCHAR(10)    NOT NULL DEFAULT 'Percent',
                    [DiscountValue]  DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    [MinOrderValue]  DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    [MaxUsageCount]  INT             NOT NULL DEFAULT 1,
                    [UsedCount]      INT             NOT NULL DEFAULT 0,
                    [StartDate]      DATETIME2       NULL,
                    [ExpiryDate]     DATETIME2       NULL,
                    [IsActive]       BIT             NOT NULL DEFAULT 1,
                    [CreatedAt]      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    CONSTRAINT [UQ_Vouchers_Code] UNIQUE ([Code])
                );
                PRINT '[SchemaUpdater] Created table Vouchers.';
            END");

        // Add StartDate column to Vouchers if missing (for existing DBs)
        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Vouchers')
               AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Vouchers') AND name = 'StartDate')
            BEGIN
                ALTER TABLE [Vouchers] ADD [StartDate] DATETIME2 NULL;
                PRINT '[SchemaUpdater] Added column StartDate to Vouchers.';
            END");

        // Feedbacks table
        await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Feedbacks')
            BEGIN
                CREATE TABLE [Feedbacks] (
                    [FeedbackId]  INT IDENTITY(1,1) PRIMARY KEY,
                    [UserId]      INT             NOT NULL,
                    [ProductId]   INT             NOT NULL,
                    [Rating]      INT             NOT NULL,
                    [Comment]     NVARCHAR(2000)  NULL,
                    [CreatedAt]   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    [UpdatedAt]   DATETIME2       NULL,
                    CONSTRAINT [FK_Feedbacks_Users]    FOREIGN KEY ([UserId])    REFERENCES [Users]([UserId])    ON DELETE CASCADE,
                    CONSTRAINT [FK_Feedbacks_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products]([ProductId]) ON DELETE CASCADE
                );
                PRINT '[SchemaUpdater] Created table Feedbacks.';
            END");

        // Add AdminReply columns to Feedbacks if missing (for existing DBs)
        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Feedbacks')
               AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Feedbacks') AND name = 'AdminReply')
            BEGIN
                ALTER TABLE [Feedbacks] ADD [AdminReply] NVARCHAR(2000) NULL;
                PRINT '[SchemaUpdater] Added column AdminReply to Feedbacks.';
            END");

        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Feedbacks')
               AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Feedbacks') AND name = 'AdminReplyAt')
            BEGIN
                ALTER TABLE [Feedbacks] ADD [AdminReplyAt] DATETIME2 NULL;
                PRINT '[SchemaUpdater] Added column AdminReplyAt to Feedbacks.';
            END");

        // Widen AvatarUrl to nvarchar(max) for Base64 image support
        await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID('Users') AND name = 'AvatarUrl'
                  AND max_length <> -1
            )
            BEGIN
                ALTER TABLE [Users] ALTER COLUMN [AvatarUrl] NVARCHAR(MAX) NULL;
                PRINT '[SchemaUpdater] Widened Users.AvatarUrl to NVARCHAR(MAX).';
            END");

        Console.WriteLine("[SchemaUpdater] New tables checked/created.");
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        var conn = _context.Database.GetDbConnection();
        var shouldClose = conn.State != ConnectionState.Open;
        if (shouldClose)
        {
            await conn.OpenAsync();
        }

        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = @name) THEN 1 ELSE 0 END";
            var nameParam = cmd.CreateParameter();
            nameParam.ParameterName = "@name";
            nameParam.Value = tableName;
            cmd.Parameters.Add(nameParam);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
        finally
        {
            if (shouldClose)
            {
                await conn.CloseAsync();
            }
        }
    }

    private async Task<bool> HasAnyUserTableAsync()
    {
        var conn = _context.Database.GetDbConnection();
        var shouldClose = conn.State != ConnectionState.Open;
        if (shouldClose)
        {
            await conn.OpenAsync();
        }

        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM sys.tables
                    WHERE is_ms_shipped = 0
                      AND name <> '__EFMigrationsHistory'
                ) THEN 1 ELSE 0 END";

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
        finally
        {
            if (shouldClose)
            {
                await conn.CloseAsync();
            }
        }
    }
}
