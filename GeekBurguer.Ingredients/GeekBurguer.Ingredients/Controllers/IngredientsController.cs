using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeekBurguer.Ingredients.Repository;

namespace GeekBurguer.Ingredients.Controllers
{
    [Route("api/products")]
    public class IngredientsController : Controller
    {
        public IngredientsController()
        {
        }

        // GET: api/Ingredients
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}