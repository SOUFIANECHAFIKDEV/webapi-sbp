using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Labels
{
    public class LabelsRepository : EntityFrameworkRepository<Entities.Labels, int>, ILabelsRepository
    {
        public LabelsRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }


        public int getNobmbreDEFois( string label)
        {
            return DbContext.Produit.Where(F => F.Labels.ToUpper().Contains("\"" + label.ToUpper() + "\"") ).Count();
        }


        public PagedList<Entities.Labels> Filter(PagingParams pagingParams, Expression<Func<Entities.Labels, bool>> filter = null, SortingParams sortingParams = null, string include = "")
        {
            IQueryable<Entities.Labels> query = this.DbContext.Set<Entities.Labels>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = BuildQueryWithEntities(query, include);

            if (sortingParams != null)
            {
                query = new SortedList<Entities.Labels>(query, sortingParams).GetSortedList();
            }

            var result=  new PagedList<Entities.Labels>(query, pagingParams.PageNumber, pagingParams.PageSize);

            result.List =  result.List.ToList().Select(x =>
            {
                x.NobmbreDEFois =  getNobmbreDEFois(x.Label);
                return x;
            }).ToList();

            return result;

        }
    }
}
