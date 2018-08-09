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
            var itemsByStore = _productRepository.ListItemsByStoreId(req.StoreId);

            if (itemsByStore.Any())
            {
                var itemsWithRestriction =
                    itemsByStore.Where(item => item.Ingredients.All(ingredient => !req.Restrictions.Contains(ingredient.Name)));

                var responseRestrictionIngredients = MapResponseRestrictionItems(itemsWithRestriction);

                if (responseRestrictionIngredients.Any())
                    return Ok(responseRestrictionIngredients);
            }

            return NotFound();
        }

        private static IEnumerable<IngredientsRestrictionsResponse> MapResponseRestrictionItems(IEnumerable<Item> itemsWithRestriction)
        {
            return itemsWithRestriction.Select(x => new IngredientsRestrictionsResponse
            {
                ProductId = x.Product.ProductId,
                Ingredients = x.Ingredients.Select(ingredient => ingredient.Name).ToList()
            });
        }
    }
}