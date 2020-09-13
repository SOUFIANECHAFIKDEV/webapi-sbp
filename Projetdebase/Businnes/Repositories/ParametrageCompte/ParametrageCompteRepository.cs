using ProjetBase.Businnes.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.ParametrageCompte


{
    public class ParametrageCompteRepository : EntityFrameworkRepository<Entities.ParametrageCompte, int>, IParametrageCompteRepository
    {

        public ParametrageCompteRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }


        public bool CheckUniqueNom(string nom)
        {
            var NbrReference = DbContext.ParametrageCompte.Where(x => x.Nom == nom ).Count();
            return NbrReference > 0;
        }


    }
}
