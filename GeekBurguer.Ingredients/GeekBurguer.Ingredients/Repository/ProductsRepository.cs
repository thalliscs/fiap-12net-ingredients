using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GeekBurguer.Ingredients.Model;

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

        public List<Item> GetFullListOfItems()
        {
            return _context.Items.ToList();
        }

        public bool Add(Product product)
        {
            product.ProductId = Guid.NewGuid();
            _context.Products.Add(product);
            return true;
        }

        public bool Update(Product product)
        {
            return true;
        }

        public IEnumerable<Product> GetProductsByStoreName(string storeName)
        {
            var products = _context.Products?
                .Where(product =>
                    product.Store.Name.Equals(storeName,
                    StringComparison.InvariantCultureIgnoreCase))
                .Include(product => product.Items);

            return products;
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }
    }
}
