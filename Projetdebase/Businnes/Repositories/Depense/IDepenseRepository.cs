using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Depense
{
    interface IDepenseRepository : IRepository<Entities.Depense, int>
    {
        Task<bool> saveMemos(int id, string memos);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Entities.Depense GetDepense(int id);
        Entities.DepenseBonCommandeFournisseur DepenseBonCommandeFournisseur(int idFicheTravail,int iddepense, Entities.BonCommandeFournisseur bonCommandeFournisseur);

    }
}
