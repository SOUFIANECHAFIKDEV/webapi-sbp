using ProjetBase.Businnes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Fournisseur
{
    interface IFournisseurRepository : IRepository<Entities.Fournisseur, int>
    {
        bool CheckUniqueReference(string reference);
        Task<bool> saveMemos(int id, string memos);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        bool supprimerFournisseur(int id);
    }
}
