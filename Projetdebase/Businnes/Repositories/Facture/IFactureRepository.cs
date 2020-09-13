using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Facture
{
    interface IFactureRepository : IRepository<Entities.Facture, int>
    {
        bool CheckUniqueReference(string reference);

        Task<bool> SaveMemos(int id, string memos);

        bool TransfertFichesInterventionToFacture(int idFacture, List<int> FichesInterventionIds);
        Task<Entities.Facture> GetFacture(int id);
        Task<object> AnnulerFacture(Entities.Facture facture, Entities.Avoir avoir);
        Task<MessagerResponseModel> SendEmail(int idFacture, InoMessagerie.Models.SendMailParamsModel MailParams);
        List<Entities.Facture> GetFacturesChantier(int IdChantier, List<int> status, DateTime? DateDebut, DateTime? DateFin);

        PagedList<Entities.Facture> Filter(PagingParams pagingParams, Expression<Func<Entities.Facture, bool>> filter = null, SortingParams sortingParams = null);

        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Task<Entities.Facture> CreateFactureSituation(FactureSituationModal factureSituationModal);
        Task<Entities.Facture> CreateFactureAcompte(FactureAcomptesModal factureAcompteModal);
       List<Entities.Facture> GetFacturesClient(int  idlient, List<int> status, DateTime? DateDebut, DateTime? DateFin);

    }
}
