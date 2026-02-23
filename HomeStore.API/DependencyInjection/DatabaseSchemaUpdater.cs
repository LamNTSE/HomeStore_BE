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
            return;
        }

        var sql = await File.ReadAllTextAsync(scriptPath);
        await _context.Database.ExecuteSqlRawAsync(sql);
        Console.WriteLine("[SchemaUpdater] Database schema updated from SQL script.");
    }
}
