using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Comptabilite
{
    interface IComptabiliteRepository
    {
        Task<PagedComptabiliteList<JournalBanqueModel>> ComptabiliteComptes(ComptabiliteComptesFilter filterModel);
        Task<PagedComptabiliteList<JournalVenteModel>> JournalVente(JournalVenteFilter filterModel);
        Task<PagedComptabiliteList<JournalVenteModel>> JournalAchat(JournalVenteFilter filterModel);
        Task<byte[]> ExportJournalVente(JournalVenteFilter filterModel);
        Task<byte[]> ExportJournalAchat(JournalVenteFilter filterModel);
        Task<byte[]> ExportComptabiliteComptes(ComptabiliteComptesFilter filterModel);
    }
}
