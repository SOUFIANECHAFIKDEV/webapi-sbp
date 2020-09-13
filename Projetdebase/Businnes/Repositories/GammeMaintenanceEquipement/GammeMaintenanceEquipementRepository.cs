using ProjetBase.Businnes.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.GammeMaintenanceEquipement
{
    public class GammeMaintenanceEquipementRepository : EntityFrameworkRepository<Entities.GammeMaintenanceEquipement,int> , IGammeMaintenanceEquipementRepository
    {
        public GammeMaintenanceEquipementRepository(ProjetBaseContext dbContext) : base(dbContext)
        {

        }

        public bool CheckUniqueNom(string nom)
        {
            var NbrReference = DbContext.Equipements.Where(x => x.Nom == nom).Count();
            return NbrReference > 0;
        }
    }
}
