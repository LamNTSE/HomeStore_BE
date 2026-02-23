using AutoMapper;
using HomeStore.Domain.DTOs.Auth;
using HomeStore.Domain.DTOs.Carts;
using HomeStore.Domain.DTOs.Chat;
using HomeStore.Domain.DTOs.Orders;
using HomeStore.Domain.DTOs.Products;
using HomeStore.Domain.DTOs.Store;
using HomeStore.Domain.Entities;

namespace HomeStore.BLL.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>();

        // Category
        CreateMap<Category, CategoryDto>();

        // Product
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null));
        CreateMap<CreateProductRequest, Product>();

        // Cart
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.CartItems));
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.ProductName : ""))
            .ForMember(d => d.ImageUrl, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImageUrl : null))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Product != null ? s.Product.Price : 0));

        // Order
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderItems))
            .ForMember(d => d.PaymentMethod, opt => opt.MapFrom(s => s.Payment != null ? s.Payment.PaymentMethod : null))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.Payment != null ? s.Payment.Status : null));
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.ProductName : ""))
            .ForMember(d => d.ImageUrl, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImageUrl : null));

        // Message
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderName, opt => opt.MapFrom(s => s.Sender != null ? s.Sender.FullName : ""))
            .ForMember(d => d.ReceiverName, opt => opt.MapFrom(s => s.Receiver != null ? s.Receiver.FullName : ""));

        // StoreLocation
        CreateMap<StoreLocation, StoreLocationDto>();
    }
}
