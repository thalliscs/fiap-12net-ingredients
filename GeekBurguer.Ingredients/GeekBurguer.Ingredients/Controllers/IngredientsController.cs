using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeekBurguer.Ingredients.Repository;
using GeekBurguer.Ingredients.Contracts;
using AutoMapper;
using GeekBurguer.Ingredients.Model;

namespace GeekBurguer.Ingredients.Controllers
{
    [Route("api/products")]
    public class IngredientsController : Controller
    {
        private IProductsRepository _productRepository;
        private IMapper _mapper;

        public IngredientsController(IProductsRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult ByRestrictions([FromBody]IngredientsRestrictionsRequest req)
        {
            var productsByStore = _productRepository.ListProductByStoreId(req.StoreId);

            if (productsByStore.Any())
            {
                var products = _mapper.Map<List<IngredientsRestrictionsResponse>>(productsByStore);

                var productsByRestriction =
                    products.Where(product =>
                    product.Ingredients.All(ingredient => !req.Restrictions.Contains(ingredient)));

                if(productsByRestriction.Any())
                    return Ok(productsByRestriction);
            }

            return NotFound();
        }
    }
}