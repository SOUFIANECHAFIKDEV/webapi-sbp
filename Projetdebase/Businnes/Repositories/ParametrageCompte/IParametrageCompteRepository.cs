using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;

namespace ProjetBase.Businnes.Repositories.ParametrageCompte
{
    public interface IParametrageCompteRepository : IRepository<Entities.ParametrageCompte, int>
    {
        bool CheckUniqueNom(string nom);

    }
}
