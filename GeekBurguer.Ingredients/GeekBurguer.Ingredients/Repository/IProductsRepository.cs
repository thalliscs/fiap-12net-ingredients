using System;
using System.Collections.Generic;
using GeekBurger.LabelLoader.Contract.Models;
using GeekBurguer.Ingredients.Model;

namespace GeekBurguer.Ingredients.Repository
{
    public interface IProductsRepository
    {
        Product GetProductById(Guid productId);
        void Add(Product product);
        void Save();
        List<Product> ListAllProducts();
        List<Product> ListProductByStoreId(Guid storeId);
        void MergeProductsAndIngredients(Produto newItem);
    }
}
