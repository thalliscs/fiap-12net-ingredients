using System;
using System.ComponentModel.DataAnnotations;

namespace GeekBurguer.Ingredients.Model
{
    public class Store
    {
        [Key]
        public Guid StoreId { get; set; }
        public string Name { get; set; }

        public Store()
        {

        }
    }
}