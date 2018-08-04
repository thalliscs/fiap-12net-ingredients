using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurguer.Ingredients.Contracts;
using GeekBurguer.Ingredients.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeekBurguer.Ingredients.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductToGet>().ReverseMap();
            CreateMap<Item, ItemToGet>().ReverseMap();
            CreateMap<Product, IngredientsRestrictionsResponse>().ReverseMap();
            
        }
    }
}
