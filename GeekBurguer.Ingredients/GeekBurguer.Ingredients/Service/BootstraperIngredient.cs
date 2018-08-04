using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurguer.Ingredients.Client;
using GeekBurguer.Ingredients.Model;
using GeekBurguer.Ingredients.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurguer.Ingredients.Service
{
    public class BootstraperIngredient : IBootstraperIngredient
    {
        private IMapper _mapper;
        private IStoreRepository _storeRepository;
        private IProductsRepository _productRepository;
        private List<Product> _productsMocked;

        public BootstraperIngredient(IMapper mapper, IStoreRepository storeRepository, IProductsRepository productRepository)
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _productsMocked = new List<Product>();
        }

        public void InitializeIngredients()
        {
            var stories = _storeRepository.GetAll();

            stories.ForEach(async store =>
            {
                try
                {
                    var response = ProductsApiClient.Current.GetProductsByStoreName(store.Name);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        string result = await response.Result.Content.ReadAsStringAsync();
                        var products = JsonConvert.DeserializeObject<List<ProductToGet>>(result);
                        PersistirProductsByStoreNameInMemory(products);
                    }
                    else
                    {
                        if (!_productsMocked.Any())
                            PopularInMemoryProductsMocked();
                    }
                }
                catch (Exception ex)
                {
                    if (!_productsMocked.Any())
                        PopularInMemoryProductsMocked();
                }
            });
        }

        private void PopularInMemoryProductsMocked()
        {
            var productsTxt = File.ReadAllText("products.json");
            var products = JsonConvert.DeserializeObject<List<Product>>(productsTxt);
            _productsMocked.AddRange(products);

            foreach (var product in _productsMocked)
            {
                if (_productRepository.GetProductById(product.ProductId) == null)
                    _productRepository.Add(product);
            }
        }

        private void PersistirProductsByStoreNameInMemory(List<ProductToGet> products)
        {
            foreach (var product in products)
            {
                var productEntity = Mapper.Map<ProductToGet, Product>(product);
                _productRepository.Add(productEntity);
            }
        }
    }
}
