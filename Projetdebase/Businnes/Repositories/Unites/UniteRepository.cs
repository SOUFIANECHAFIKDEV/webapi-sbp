using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Unites
{
    public class UniteRepository : EntityFrameworkRepository<Unite, int>, IUniteRepository
    {
        public UniteRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}
