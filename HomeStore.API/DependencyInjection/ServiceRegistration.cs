using HomeStore.BLL.Mappings;
using HomeStore.BLL.Services;
using HomeStore.DAL;
using HomeStore.DAL.Repositories;
using HomeStore.Domain.Interfaces.Repositories;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.API.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<HomeStoreV2Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IStoreLocationRepository, StoreLocationRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IVoucherService, VoucherService>();
        services.AddScoped<IFeedbackService, FeedbackService>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Schema updater & seeder
        services.AddScoped<IDatabaseSchemaUpdater, DatabaseSchemaUpdater>();
        services.AddScoped<IDatabaseDataSeeder, DatabaseDataSeeder>();

        return services;
    }
}
