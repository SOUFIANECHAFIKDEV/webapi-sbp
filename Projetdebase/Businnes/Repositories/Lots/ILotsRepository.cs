using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Lots
{

    public interface ILotsRepository : IRepository<Entities.Lots, int>
    {
        PagedList<Entities.Lots> Filter(PagingParams pagingParams, Expression<Func<Entities.Lots, bool>> filter = null, SortingParams sortingParams = null);
        bool supprimerLots(int id);
        bool CheckUniqueReference(string nom);
        void CreateLots(Entities.Lots entity);
    }
}
