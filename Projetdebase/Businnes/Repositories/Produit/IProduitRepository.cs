using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static ProjetBase.Businnes.Models.FicheTechniqueModal;

namespace ProjetBase.Businnes.Repositories.Produit
{
    interface IProduitRepository : IRepository<Entities.Produit, int>
    {
        bool CheckUniqueReference(string reference);
        Task<bool> saveMemos(int id, string memos);
        Task<IList<Tva>> getListeTva();
        Task<Tva> getTvaById(int idTva);
        Task<IList<Unite>> getListeUnite();
        Task<Unite> getUniteById(int idUnite);
        Task<IList<Entities.Categorie>> getListeCategorie();
        Task<Entities.Categorie> getCategorieById(int idCategorie);
        bool supprimerProduit(int id);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        PagedList<Entities.Produit> Filter(int? IdFournisseur,PagingParams pagingParams, Expression<Func<Entities.Produit, bool>> filter = null, SortingParams sortingParams = null);
        Entities.Produit GetProduit(int id);
        Task<bool> deletePrixParFournisseur(List<ProduitFournisseur> prixParFournisseurs);
        Task<bool> updateFicheTehcnique(int id, List<ficheTechniqueModal> ficheTechniques);
    }
}
