using InoAuthentification.Entities;
using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.BonCommandeFournisseur
{
    interface IBonCommandeFournisseur : IRepository<Entities.BonCommandeFournisseur, int>
    {
        bool CheckUniqueReference(string reference);
        Task<bool> saveMemos(int id, string memos);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Entities.BonCommandeFournisseur GetBonCommandeFournisseur(int id);

    }
}
