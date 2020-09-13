using ProjetBase.Businnes.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Rubrique
{
    public class RubriqueRepository : EntityFrameworkRepository<Entities.Rubrique, int>, IRubriqueRepository
    {

        public RubriqueRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}
