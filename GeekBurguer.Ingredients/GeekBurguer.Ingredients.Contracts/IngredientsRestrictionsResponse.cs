using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurguer.Ingredients.Contracts
{
    public class IngredientsRestrictionsResponse
    {
        public Guid ProductId { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>();
    }
}
