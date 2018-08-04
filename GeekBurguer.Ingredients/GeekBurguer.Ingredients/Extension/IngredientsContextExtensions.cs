using GeekBurguer.Ingredients.Model;
using GeekBurguer.Ingredients.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurguer.Ingredients.Extension
{
    public static class IngredientsContextExtensions
    {
        public static void Seed(this IngredientsContext context)
        {
            context.Items.RemoveRange(context.Items);
            context.Products.RemoveRange(context.Products);
            context.Stores.RemoveRange(context.Stores);

            context.SaveChanges();

            PopularDadosStores(context);
        }

        private static void PopularDadosStores(IngredientsContext context)
        {
            context.Stores.AddRange(new List<Store> {
                new Store { Name = "Los Angeles - Pasadena", StoreId = new Guid("8048e9ec-80fe-4bad-bc2a-e4f4a75c834e") },
                new Store { Name = "Los Angeles - Beverly Hills", StoreId = new Guid("8d618778-85d7-411e-878b-846a8eef30c0") }
            });

            context.SaveChanges();
        }
    }
}
