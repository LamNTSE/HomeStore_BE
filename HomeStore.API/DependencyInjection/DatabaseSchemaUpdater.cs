using HomeStore.DAL;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.API.DependencyInjection;

public class DatabaseSchemaUpdater : IDatabaseSchemaUpdater
{
    private readonly HomeStoreV2Context _context;
    private readonly IWebHostEnvironment _env;

    public DatabaseSchemaUpdater(HomeStoreV2Context context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task UpdateSchemaAsync()
    {
        var scriptPath = Path.Combine(_env.ContentRootPath, "DatabaseScripts", "01_HomeStore_Schema.sql");
        if (!File.Exists(scriptPath))
        {
            Console.WriteLine($"[SchemaUpdater] Script not found: {scriptPath}. Using EF EnsureCreated instead.");
            await _context.Database.EnsureCreatedAsync();
        }
        else
        {
            var sql = await File.ReadAllTextAsync(scriptPath);
            await _context.Database.ExecuteSqlRawAsync(sql);
            Console.WriteLine("[SchemaUpdater] Database schema updated from SQL script.");
        }

        // Ensure new tables added after initial schema are created
        await EnsureNewTablesAsync();
    }

    private async Task EnsureNewTablesAsync()
    {
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

        Console.WriteLine("[SchemaUpdater] New tables checked/created.");
    }
}
