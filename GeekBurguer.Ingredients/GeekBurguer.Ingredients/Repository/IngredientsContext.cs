﻿using GeekBurguer.Ingredients.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekBurguer.Ingredients.Repository
{
    public class IngredientsContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseInMemoryDatabase("geekburger-ingredients");
    }
}
