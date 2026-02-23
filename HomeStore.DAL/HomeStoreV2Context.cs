using HomeStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL;

public class HomeStoreV2Context : DbContext
{
    public HomeStoreV2Context(DbContextOptions<HomeStoreV2Context> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<StoreLocation> StoreLocations => Set<StoreLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        // Cart: one-to-one with User
        modelBuilder.Entity<Cart>(e =>
        {
            e.HasOne(c => c.User)
             .WithOne(u => u.Cart)
             .HasForeignKey<Cart>(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem
        modelBuilder.Entity<CartItem>(e =>
        {
            e.HasOne(ci => ci.Cart)
             .WithMany(c => c.CartItems)
             .HasForeignKey(ci => ci.CartId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ci => ci.Product)
             .WithMany(p => p.CartItems)
             .HasForeignKey(ci => ci.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.HasOne(o => o.User)
             .WithMany(u => u.Orders)
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasOne(oi => oi.Order)
             .WithMany(o => o.OrderItems)
             .HasForeignKey(oi => oi.OrderId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(oi => oi.Product)
             .WithMany(p => p.OrderItems)
             .HasForeignKey(oi => oi.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment: one-to-one with Order
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasOne(p => p.Order)
             .WithOne(o => o.Payment)
             .HasForeignKey<Payment>(p => p.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Message
        modelBuilder.Entity<Message>(e =>
        {
            e.HasOne(m => m.Sender)
             .WithMany(u => u.SentMessages)
             .HasForeignKey(m => m.SenderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Receiver)
             .WithMany(u => u.ReceivedMessages)
             .HasForeignKey(m => m.ReceiverId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
