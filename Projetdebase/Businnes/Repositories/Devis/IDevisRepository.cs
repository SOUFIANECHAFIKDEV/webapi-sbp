using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Devis
{
    interface IDevisRepository : IRepository<Entities.Devis, int>
    {
        bool CheckUniqueReference(string reference);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Entities.Devis GetDevis(int id);
        Task<MessagerResponseModel> SendEmail(int idDevis, InoMessagerie.Models.SendMailParamsModel MailParams);
        Task<bool> changerStatutChantier(Entities.Devis devis);
        Entities.BonCommandeFournisseur createBCObjet(Entities.Devis devis, int IDFOURNISSEUR, List<Data> prestation);
        bool supprimerDevis(int id);
    }
}
