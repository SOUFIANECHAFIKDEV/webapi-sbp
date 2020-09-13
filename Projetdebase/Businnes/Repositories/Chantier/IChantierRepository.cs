using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Chantier
{

    public interface IChantierRepository : IRepository<Entities.Chantier, int>
    {
        bool CheckUniqueName(string nom);
        bool supprimerChantier(int id);
        Task<List<Entities.Chantier>> GetAllChantier();
        Task<Entities.Chantier> GetChantier(int id);
        Models.ChantierChangeStatusResponse changeStatut(Models.ChangeStatutBodyRequest body);

        Models.nbDocumentsChantieModel nbDocuments(int idChantier);
        Entities.Chantier getChantier(int id);
        RecapitulatifFinancierModel GetRecapitulatifFinancier(int idChantier);
        List<RetenueGarantieModel> GetRetenueGarantie(int idChantier);
    }
}
