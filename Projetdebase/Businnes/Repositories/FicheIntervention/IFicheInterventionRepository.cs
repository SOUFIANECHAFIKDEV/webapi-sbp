using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.FicheIntervention
{
    interface IFicheInterventionRepository : IRepository<Entities.FicheIntervention, int>
    {
        bool CheckUniqueReference(string reference);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        Entities.FicheIntervention GetFicheIntervention(int id);
        Task<MessagerResponseModel> SendEmail(int idFicheIntervention, InoMessagerie.Models.SendMailParamsModel MailParams);
        string AddEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention);
        string UpdateEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention);
        string DeleteEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention);
        Task<bool> saveMemos(int id, string memos);
        Task<List<Entities.FicheIntervention>> GetFicheInterventionsChantier(int IdClient);
        PagedList<Entities.FicheIntervention> Filter(PagingParams pagingParams, Expression<Func<Entities.FicheIntervention, bool>> filter = null, SortingParams sortingParams = null);
    }
}
