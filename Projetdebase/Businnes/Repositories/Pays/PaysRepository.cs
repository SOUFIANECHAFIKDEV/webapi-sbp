using ProjetBase.Businnes.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Pays
{
    public class PaysRepository : EntityFrameworkRepository<Entities.Pays, int>, IPaysRepository
    {

        public PaysRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}
