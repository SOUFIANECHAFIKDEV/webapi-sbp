using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.FicheInterventionMaintenance
{
    interface IFicheInterventionMaintenanceRepository : IRepository<Entities.FicheInterventionMaintenance, int>
    {
        bool CheckUniqueReference(string reference);
        Entities.FicheInterventionMaintenance GetFicheInterventionMaintenance(int id);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
    }
}
