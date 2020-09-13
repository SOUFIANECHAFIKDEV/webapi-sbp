using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.ContratEntretien
{
    interface IContratEntretienRepository : IRepository<Entities.ContratEntretien, int>
    {
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Task<bool> saveMemos(int id, string memos);
        Task<byte[]> ExportGammeMaintenenceEquipement(Entities.ContratEntretien contratEntretien);
        Entities.ContratEntretien GetContratEntretien(int id);
        Task<bool> deleteEquipementContrat(List<EquipementContrat> equipementContrat);
        Task<List<EquipementContrat>> AddEquipementContrat(int id, List<EquipementContrat> equipementContrat);
    }
}
