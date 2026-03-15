using HomeStore.DAL;
using HomeStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.API.DependencyInjection;

public class DatabaseDataSeeder : IDatabaseDataSeeder
{
    private readonly HomeStoreV2Context _context;

    public DatabaseDataSeeder(HomeStoreV2Context context) => _context = context;

    public async Task SeedAsync()
    {
        Console.WriteLine("Seeder started");

        await SeedUsersAsync();
        await SeedCategoriesAndProductsAsync();
        await SeedVouchersAsync();
        

        Console.WriteLine("Seeder finished");
    }

    // ── Users ────────────────────────────────────────────────────────────────
    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        _context.Users.AddRange(new List<User>
        {
            new()
            {
                FullName = "Admin HomeStore",
                Email = "admin@homestore.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Phone = "0901234567",
                Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                Role = "Admin",
                Provider = "Local"
            },
            new()
            {
                FullName = "Nguyễn Văn A",
                Email = "nguyenvana@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Phone = "0912345678",
                Address = "456 Lê Lợi, Quận 1, TP.HCM",
                Role = "Customer",
                Provider = "Local"
            },
            new()
            {
                FullName = "Trần Thị B",
                Email = "tranthib@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Phone = "0923456789",
                Address = "789 Hai Bà Trưng, Quận 3, TP.HCM",
                Role = "Customer",
                Provider = "Local"
            }
        });

        await _context.SaveChangesAsync();
        Console.WriteLine("[DataSeeder] Seeded users.");
    }

    // ── Categories, Products, StoreLocations ─────────────────────────────────
    private async Task SeedCategoriesAndProductsAsync()
    {
        if (await _context.Categories.AnyAsync()) return;

        var categories = new List<Category>
        {
            new() { CategoryName = "Living Room", Description = "Furniture for the living room", ImageUrl = "https://via.placeholder.com/200?text=LivingRoom" },
            new() { CategoryName = "Bedroom", Description = "Furniture for the bedroom", ImageUrl = "https://via.placeholder.com/200?text=Bedroom" },
            new() { CategoryName = "Kitchen", Description = "Kitchen furniture and appliances", ImageUrl = "https://via.placeholder.com/200?text=Kitchen" },
            new() { CategoryName = "Bathroom", Description = "Bathroom accessories", ImageUrl = "https://via.placeholder.com/200?text=Bathroom" },
            new() { CategoryName = "Office", Description = "Office furniture", ImageUrl = "https://via.placeholder.com/200?text=Office" }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();

        _context.Products.AddRange(new List<Product>
        {
            new() { ProductName = "Modern Sofa",      Description = "Comfortable 3-seat sofa",       Price = 12500000, StockQuantity = 10, CategoryId = categories[0].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Sofa" },
            new() { ProductName = "Coffee Table",     Description = "Wooden coffee table",            Price =  3500000, StockQuantity = 20, CategoryId = categories[0].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=CoffeeTable" },
            new() { ProductName = "King Size Bed",    Description = "King size wooden bed frame",     Price = 18000000, StockQuantity =  5, CategoryId = categories[1].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Bed" },
            new() { ProductName = "Wardrobe",         Description = "Large wardrobe with mirror",     Price =  9500000, StockQuantity =  8, CategoryId = categories[1].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Wardrobe" },
            new() { ProductName = "Dining Table Set", Description = "6-chair dining table set",       Price = 15000000, StockQuantity =  7, CategoryId = categories[2].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=DiningTable" },
            new() { ProductName = "Kitchen Cabinet",  Description = "Modern kitchen cabinet",         Price =  8000000, StockQuantity = 12, CategoryId = categories[2].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Cabinet" },
            new() { ProductName = "Bathroom Mirror",  Description = "LED bathroom mirror",            Price =  2500000, StockQuantity = 15, CategoryId = categories[3].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Mirror" },
            new() { ProductName = "Office Desk",      Description = "Ergonomic office desk",          Price =  6500000, StockQuantity = 10, CategoryId = categories[4].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Desk" },
            new() { ProductName = "Office Chair",     Description = "Ergonomic office chair",         Price =  4500000, StockQuantity = 15, CategoryId = categories[4].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Chair" },
            new() { ProductName = "Bookshelf",        Description = "5-tier wooden bookshelf",        Price =  3200000, StockQuantity = 20, CategoryId = categories[4].CategoryId, ImageUrl = "https://via.placeholder.com/300?text=Bookshelf" }
        });

        _context.StoreLocations.AddRange(new List<StoreLocation>
        {
            new() { StoreName = "HomeStore Quận 1", Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",          Latitude = 10.7769, Longitude = 106.7009, Phone = "028-1234-5678" },
            new() { StoreName = "HomeStore Quận 7", Address = "456 Nguyễn Thị Thập, Quận 7, TP.HCM",     Latitude = 10.7340, Longitude = 106.7218, Phone = "028-8765-4321" }
        });

        await _context.SaveChangesAsync();
        Console.WriteLine("[DataSeeder] Seeded categories, products, and store locations.");
    }

    // ── Vouchers ─────────────────────────────────────────────────────────────
    private async Task SeedVouchersAsync()
    {
        if (await _context.Vouchers.AnyAsync()) return;

        _context.Vouchers.AddRange(new List<Voucher>
        {
            new()
            {
                Code = "WELCOME10",
                DiscountType = "Percent",
                DiscountValue = 10,
                MinOrderValue = 500000,
                MaxUsageCount = 100,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsActive = true
            },
            new()
            {
                Code = "SUMMER20",
                DiscountType = "Percent",
                DiscountValue = 20,
                MinOrderValue = 2000000,
                MaxUsageCount = 50,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                IsActive = true
            },
            new()
            {
                Code = "SAVE200K",
                DiscountType = "Fixed",
                DiscountValue = 200000,
                MinOrderValue = 1000000,
                MaxUsageCount = 200,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(12),
                IsActive = true
            },
            new()
            {
                Code = "FLASH50",
                DiscountType = "Percent",
                DiscountValue = 50,
                MinOrderValue = 5000000,
                MaxUsageCount = 10,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsActive = true
            },
            new()
            {
                // Voucher sắp có hiệu lực (chưa đến StartDate)
                Code = "NEWYEAR30",
                DiscountType = "Percent",
                DiscountValue = 30,
                MinOrderValue = 3000000,
                MaxUsageCount = 20,
                StartDate = DateTime.UtcNow.AddDays(7),
                ExpiryDate = DateTime.UtcNow.AddDays(37),
                IsActive = true
            },
            new()
            {
                // Voucher đã hết hạn/hết lượt
                Code = "OLDVOUCHER",
                DiscountType = "Percent",
                DiscountValue = 15,
                MinOrderValue = 0,
                MaxUsageCount = 30,
                UsedCount = 30,
                StartDate = DateTime.UtcNow.AddDays(-30),
                ExpiryDate = DateTime.UtcNow.AddDays(-1),
                IsActive = false
            },
            new()
            {
                // Voucher không giới hạn thời gian — dùng lâu dài
                Code = "FOREVER5",
                DiscountType = "Percent",
                DiscountValue = 5,
                MinOrderValue = 0,
                MaxUsageCount = 99999,
                StartDate = null,
                ExpiryDate = null,
                IsActive = true
            }
        });

        await _context.SaveChangesAsync();
        Console.WriteLine("[DataSeeder] Seeded vouchers.");
    }
}
