using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.PeriodeComptable
{
    interface IPeriodeComptablesRepository : IRepository<Entities.PeriodeComptable, int>
    {
        Task CloturePeriode(int Id);
    }
}
