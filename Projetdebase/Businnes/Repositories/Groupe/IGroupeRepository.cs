using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Groupe
{
    interface IGroupeRepository : IRepository<Entities.Groupe, int>
    {
        bool CheckUniqueNom(string nom);
        Entities.Groupe GetGroupe(int id);

    }
}
