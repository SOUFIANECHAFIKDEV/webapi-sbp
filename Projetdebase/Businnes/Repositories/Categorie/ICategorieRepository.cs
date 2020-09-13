using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Categorie
{
    public interface ICategorieRepository : IRepository<Entities.Categorie, int> 
    {
        bool CheckUniqueNom(string nom);
    }
}
