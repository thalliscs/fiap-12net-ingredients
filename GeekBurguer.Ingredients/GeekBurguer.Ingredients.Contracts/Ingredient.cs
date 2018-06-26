using System;
using System.Collections.Generic;

namespace GeekBurguer.Ingredients.Contracts
{
    public class MergeProductsAndIngredients
    {
        public Guid ProductId { get; set; }
        public List<string> Ingredients { get; set; }
        public Guid StoreId { get; set; }
    }

    public class GenerateIngredientsList
    {
        public Guid ItemId { get; set; }
        public List<string> Ingredients { get; set; }
    }

}