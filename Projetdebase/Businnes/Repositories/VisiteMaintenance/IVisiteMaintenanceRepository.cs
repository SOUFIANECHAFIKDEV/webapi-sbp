using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.VisiteMaintenance
{
    interface IVisiteMaintenanceRepository : IRepository<Entities.VisiteMaintenance, int>
    {
        Task<bool> AddVisiteMaintenance(Entities.ContratEntretien contratEntretien);
        Entities.VisiteMaintenance GetVisiteMaintenance(int id);
       // PagedList<Entities.VisiteMaintenance> Filter(PagingParams pagingParams, Expression<Func<Entities.VisiteMaintenance, bool>> filter = null, SortingParams sortingParams = null);
        PagedList<Entities.VisiteMaintenance> Filter(PagingParams pagingParams, Expression<Func<Entities.VisiteMaintenance, bool>> filter = null, SortingParams sortingParams = null);


    }
}
