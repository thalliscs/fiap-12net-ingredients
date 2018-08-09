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

        public ProductsRepository(IngredientsContext context) => _context = context;

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

        public List<Item> ListItemsByStoreId(Guid storeId)
        {
            return _context.Items?
                .Include(x=>x.Ingredients)
                .Include(x => x.Product)
                .ThenInclude(x=>x.Store)
                .Where(x => x.Product.StoreId == storeId).ToList();
        }

        public List<Item> ListProductByIngredientName(string name)
        {
            List<Item>  items = _context.Items?
              .Include(item => item.Ingredients).Where(ingredient => ingredient.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return items;
        }

        public Item GetItemById(Guid itemId)
        {
            return _context.Items?.Include(item => item.Ingredients).FirstOrDefault(x => x.ItemId == itemId);
        }
    }
}
