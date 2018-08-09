using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using GeekBurguer.Ingredients.Repository;
using AutoMapper;
using GeekBurguer.Ingredients.Extension;
using GeekBurguer.Ingredients.Service;
using System.Threading.Tasks;
using System;

namespace GeekBurguer.Ingredients
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            var mvcCoreBuilder = services.AddMvcCore();

            mvcCoreBuilder
                .AddApiExplorer()
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Ingredients", Version = "v1" });
            });

            services.AddAutoMapper();

            services.AddScoped<IngredientsContext>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddSingleton<ILabelImageAddedService, LabelImageAddedService>();
            services.AddSingleton<IBootstraperIngredient, BootstraperIngredient>();
            services.AddSingleton<ILogService, LogService>();

            await InitializeIngredients(services);

             services.AddMvc();
        }

        private async Task InitializeIngredients(IServiceCollection services)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            var boostraperIngredient = provider.GetService<IBootstraperIngredient>();
            var labelImageAddedService = provider.GetService<ILabelImageAddedService>();
            var ingredientsContext = provider.GetService<IngredientsContext>();
            ingredientsContext.Seed();

            await boostraperIngredient.InitializeIngredients();
            await labelImageAddedService.ReceiveMessages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
               "Ingredients");
            });

            app.UseMvc();
        }
    }
}
