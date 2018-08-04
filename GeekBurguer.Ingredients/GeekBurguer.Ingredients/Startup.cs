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
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcCoreBuilder = services.AddMvcCore();

            mvcCoreBuilder
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Ingredients", Version = "v1" });
            });

            services.AddAutoMapper();

            services.AddDbContext<IngredientsContext>(o => o.UseInMemoryDatabase("geekburger-ingredients"));
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IBootstraperIngredient, BootstraperIngredient>();
            services.AddScoped<ILabelImageAddedService, LabelImageAddedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IngredientsContext ingredientsContext, IBootstraperIngredient boostraperIngredient,
            ILabelImageAddedService labelImageAddedService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
               "Ingredients");
            });

            ingredientsContext.Seed();
            boostraperIngredient.InitializeIngredients();
            labelImageAddedService.ReceiveAsync();
        }
    }
}
