﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekBurguer.Ingredients.Model
{
    public class Item
    {
        [Key]
        public Guid ItemId { get; set; }
        public string Name { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public Item()
        {

        }

        public void AddIngredients(List<string> lstIngredients)
        {

            lstIngredients.ForEach(ingredient =>
                Ingredients.Add(new Ingredient() { Name = ingredient }));
        }
    }
}
