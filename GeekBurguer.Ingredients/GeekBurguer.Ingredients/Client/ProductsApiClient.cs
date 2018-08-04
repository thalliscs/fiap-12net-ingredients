using GeekBurger.Products.Contract;
using GeekBurguer.Ingredients.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeekBurguer.Ingredients.Client
{
    class ProductsApiClient
    {
        private static Lazy<ProductsApiClient> _Lazy = new Lazy<ProductsApiClient>(() => new ProductsApiClient());
        public static ProductsApiClient Current { get => _Lazy.Value; }

        private ProductsApiClient()
        {
            _HttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://geekburgerproducts.azurewebsites.net/")
            };
        }

        private readonly HttpClient _HttpClient;

        public async Task<HttpResponseMessage> GetProductsByStoreName(string storeName)
        {
            List<ProductToGet> products = new List<ProductToGet>();
            try
            {
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await _HttpClient.GetAsync($"api/products?storeName={storeName}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
