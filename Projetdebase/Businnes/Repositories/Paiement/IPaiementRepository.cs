using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Paiement
{
    public interface IPaiementRepository : IRepository<Entities.Paiement, int>
    {
      

        Task<Entities.Paiement> Save(Entities.Paiement paiement);
        Task<Entities.Paiement> SavePaiementDepense(Entities.Paiement paiement);
        Task<Entities.Paiement> GetPaiement(int id);
        Task UpdatePaiement(Entities.Paiement paiement);
        Task UpdatePaiementDepense(Entities.Paiement paiement);
        void DeletePaiement(Entities.Paiement paiement);
        void DeletePaiementDepense(Entities.Paiement paiement);
        void CheckFactureStatutAfterPaiement(List<int> idsFacture);
        List<ModifyEntryModel> Historique(Entities.Paiement paiement, Entities.Paiement paiementDB);
        Task<bool> MovementCompteACompte(PaiementMovementCompteACompteModel paiementMovementCompte);
        Task<double> TotalPaiement(PaiementModel paiementModel);
        Task<bool> CheckValidePaiementAjout(List<FacturePaiement> facturePaiements);
        Task<bool> CheckValidePaimentEdit(List<FacturePaiement> facturePaiements);
        Task<bool> CheckValidePaiementDepenseAjout(Entities.Paiement Paiementss);
        Task<bool> CheckValidePaimentDepenseEdit(Entities.Paiement Paiements);
        void changeStatutToTermine(int idChantier);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);

    }
}
