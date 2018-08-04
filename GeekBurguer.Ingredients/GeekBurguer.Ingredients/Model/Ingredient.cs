using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurguer.Ingredients.Model
{
    public class Ingredient
    {
        [Key]
        public Guid IngredientId { get; set; }
        public string Name { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        public Ingredient()
        {

        }
    }
}
