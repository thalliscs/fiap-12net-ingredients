using GeekBurguer.Ingredients.Model;
using System.Collections.Generic;

namespace GeekBurguer.Ingredients.Repository
{
    public interface IStoreRepository
    {
        List<Store> GetAll();
    }
}
