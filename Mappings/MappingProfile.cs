using AutoMapper;
using Getdata1.Models;
using Getdata1.DTOs;

namespace Getdata1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product Mapping
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.GalleryImages, opt => opt.MapFrom(src => src.ProductImages != null ? src.ProductImages.Select(pi => pi.Url).Where(u => u != null).Cast<string>().ToList() : new List<string>()))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ProductReviews));

            // ProductReview Mapping
            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : src.UserEmail));

            // Category Mapping
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            // Order Mapping
            CreateMap<_Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderItems.Sum(i => i.Price * i.Quantity)))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => src.ShippingMethod))
                .ForMember(dest => dest.ShippingFee, opt => opt.MapFrom(src => src.ShippingFee))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.PromoCode, opt => opt.MapFrom(src => src.PromoCode));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products != null ? src.Products.Name : "Unknown"))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Products != null ? src.Products.Image : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Products != null && src.Products.Category != null ? src.Products.Category.Name : "Sản phẩm"));
        }
    }
}