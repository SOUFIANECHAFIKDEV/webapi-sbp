using ProjetBase.Businnes.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Departement
{
    public class DepartementRepository : EntityFrameworkRepository<Entities.Departement, int>, IDepartmentRepository
    {

        public DepartementRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}
