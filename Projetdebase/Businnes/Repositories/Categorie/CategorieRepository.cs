using ProjetBase.Businnes.Contexts;
using System;
using InoAuthentification.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Categorie
{
    public class CategorieRepository : EntityFrameworkRepository<Entities.Categorie, int>, ICategorieRepository
    {
        public CategorieRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }

        public bool CheckUniqueNom(string nom)
        {
            var NbrReference = DbContext.Categorie.Where(x => x.Nom == nom).Count();
            return NbrReference > 0;
        }
    }
}
