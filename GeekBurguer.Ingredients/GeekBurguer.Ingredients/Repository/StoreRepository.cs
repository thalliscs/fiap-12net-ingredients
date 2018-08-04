using GeekBurguer.Ingredients.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeekBurguer.Ingredients.Repository
{
    public class StoreRepository : IStoreRepository
    {
        private IngredientsContext _context { get; set; }

        public StoreRepository(IngredientsContext context)
        {
            _context = context;
        }

        public List<Store> GetAll()
        {
            return _context.Stores.ToList();
        }
    }
}
