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
            var itensMocked = _productRepository.ListProductByStoreId(req.StoreId);

            if (itensMocked.Count <= 0)
                return NotFound();

            var productsMockedToGet = _mapper.Map<List<IngredientsRestrictionsResponse>>(itensMocked);
            foreach (var productMocked in productsMockedToGet)
            {
                productMocked.Ingredients.Add("soy mocked");
                productMocked.Ingredients.Add("gluten mocked");
            }
            
            return Ok(productsMockedToGet);
        }
    }
}