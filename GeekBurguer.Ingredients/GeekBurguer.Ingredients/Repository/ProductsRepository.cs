using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GeekBurguer.Ingredients.Model;
using GeekBurger.LabelLocader.Contract.Models;

namespace GeekBurguer.Ingredients.Repository
{

    public class ProductsRepository : IProductsRepository
    {
        private IngredientsContext _context;

        public ProductsRepository(IngredientsContext context)
        {
            _context = context;
        }

        public Product GetProductById(Guid productId)
        {
            return _context.Products?
                .Include(product => product.Items)
                .FirstOrDefault(product => product.ProductId == productId);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Product> ListAllProducts()
        {
            return _context.Products?
                .Include(product => product.Items)
                .Include(product => product.Store)
                .ToList();
        }

        public List<Product> ListProductByStoreId(Guid storeId)
        {
            return _context.Products?
                .Include(product => product.Items)
                .Include(product => product.Store)
                .Where(x => x.StoreId == storeId).ToList();
        }

        public void MergeProductsAndIngredients(Produto newItem)
        {
            if (newItem != null)
                ListAllProducts().ForEach(product =>
                    product.Items.Where(items => items.Name == newItem.ItemName).ToList()
                        .ForEach(item => item.AddIngredients(newItem.Ingredients)));

            _context.SaveChanges();
        }
    }
}
