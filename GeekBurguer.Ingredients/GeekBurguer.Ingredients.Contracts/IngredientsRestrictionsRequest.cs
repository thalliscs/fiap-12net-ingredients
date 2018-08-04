using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurguer.Ingredients.Contracts
{
    public class IngredientsRestrictionsRequest
    {
        public List<string> Restrictions { get; set; }
        public Guid StoreId { get; set; }
    }
}
