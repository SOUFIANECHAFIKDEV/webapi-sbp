using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.GammeMaintenanceEquipement
{
    interface IGammeMaintenanceEquipementRepository : IRepository<Entities.GammeMaintenanceEquipement, int>
    {
        bool CheckUniqueNom(string nom);
    }
}
