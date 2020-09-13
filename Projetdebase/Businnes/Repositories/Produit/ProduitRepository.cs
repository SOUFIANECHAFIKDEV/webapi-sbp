using InovaFileManager;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static ProjetBase.Businnes.Models.FicheTechniqueModal;

namespace ProjetBase.Businnes.Repositories.Produit
{
    public class ProduitRepository : EntityFrameworkRepository<Entities.Produit, int>, IProduitRepository
    {
        public ProjetBaseContext SbpContext;
        private readonly FileManager fileManager;
        public ProduitRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
            this.fileManager = new FileManager();
        }

        //verifier que la reference de produit est unique
        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.Produit.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        //sauvegarder les mémos de produit
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var produit = GetById(id);
                produit.FichesTechniques = memos;
                Update(produit);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        //récupérer la liste des tva
        public async Task<IList<Tva>> getListeTva()
        {
            return DbContext.Tva.ToList();
        }

        //récupérer la tva par id
        public async Task<Tva> getTvaById(int idTva)
        {
            return DbContext.Tva.Where(T => T.Id == idTva).FirstOrDefault();
        }

        //récupérer l'Unite de produit par id
        public async Task<IList<Entities.Unite>> getListeUnite()
        {
            return DbContext.Unite.ToList();
        }

        //récupérer l'Unite de produit par id
        public async Task<Unite> getUniteById(int idUnite)
        {
            return DbContext.Unite.Where(T => T.Id == idUnite).FirstOrDefault();
        }

        //récupérer la Categorie de produit par id
        public async Task<IList<Entities.Categorie>> getListeCategorie()
        {
            try
            {
                var categorie = DbContext.Categorie.ToList();
                return categorie;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //récupérer la Categorie de produit par id
        public async Task<Entities.Categorie> getCategorieById(int idCategorie)
        {
            return DbContext.Categorie.Where(T => T.Id == idCategorie).FirstOrDefault();
        }

        public bool supprimerProduit(int id)
        {
            if (DbContext.LotProduits.Where(c => c.IdProduit == id).Count() != 0)
            {
                return false;
            }
            var Produit = DbContext.Produit.SingleOrDefault(C => C.Id == id);
            DbContext.Remove(Produit);
            DbContext.SaveChanges();
            return true;
        }
        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>
                {

                    if (h.Attribute != "Labels" && h.Attribute != "Id" && h.Attribute != "Historique")
                    {
                        if (h.Attribute == "PrixParFournisseur")
                        {

                            //string after = "";
                            //List<ProduitFournisseur> afterData = JsonConvert.DeserializeObject<List<ProduitFournisseur>>(h.After);
                            //foreach (ProduitFournisseur A in afterData)
                            //{
                            //    var newLine = "";
                            //    newLine = String.Concat(after, ("<strong>Fournisseur</strong> : {0} </br>", DbContext.Fournisseurs.SingleOrDefault(w => w.Id == A.idFournisseur).Nom));
                            //    newLine = String.Concat(after, ("<strong>Prix</strong> : {0} </br>", A.prix));
                            //    newLine = String.Concat(after, ("<strong>Par default</strong> : {0} </br></hr></br>", (A.@default == 0 ? "Non" : "Oui")));
                            //    //("<strong>Produit</strong> : {0} ", after);
                            //    after = String.Concat(after, newLine);
                            //}

                            //string before = "";
                            //List<ProduitFournisseur> beforeData = JsonConvert.DeserializeObject<List<ProduitFournisseur>>(h.Before);
                            //foreach (ProduitFournisseur A in beforeData)
                            //{
                            //    var newLine = "";
                            //    newLine = String.Concat(before, ("<strong>Fournisseur</strong> : {0} </br>", DbContext.Fournisseurs.SingleOrDefault(w => w.Id == A.idFournisseur).Nom));
                            //    newLine = String.Concat(before, ("<strong>Prix</strong> : {0} </br>", A.prix));
                            //    newLine = String.Concat(before, ("<strong>Par default</strong> : {0} </br></hr></br>", (A.@default == 0 ? "Non" : "Oui")));
                            //    before = String.Concat(before, newLine);
                            //}


                            NewhistoriqueChamps.Add(new ModifyEntryModel
                            {
                                Attribute = "Prix par fournisseur",
                                After = "<small style=\"color:red\">Données non disponibles</small>",
                                Before = "<small style=\"color:red\">Données non disponibles</small>"
                                //Before = "this is my \"data\" in the string"
                            });
                        }
                        else
                        {
                            NewhistoriqueChamps.Add(h);
                        }

                    }
                });

                return NewhistoriqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public PagedList<Entities.Produit> Filter(int? IdFournisseur, PagingParams pagingParams, Expression<Func<Entities.Produit, bool>> filter = null, SortingParams sortingParams = null)
        {
            try
            {
                IQueryable<Entities.Produit> query = DbContext.Set<Entities.Produit>();
                query = query.Where(filter).Include(f => f.PrixParFournisseur);
                //query = query.Where(x => x.PrixParFournisseur.Where(f => (IdFournisseur.HasValue ? ((f.idFournisseur == IdFournisseur) && (f.@default == 1)) : true)).Count() > 0);
                query = query.Where(x =>
                IdFournisseur.HasValue ?
                (x.PrixParFournisseur.Where(f => f.idFournisseur == IdFournisseur).Count() > 0)
                : true);

                //x => x.PrixParFournisseur.Where(X => IdFournisseur.HasValue ? ((X.idFournisseur == IdFournisseur) : true))).Count()

                //if (sortingParams != null)
                //{
                //    query = new SortedList<Entities.Produit>(query, sortingParams).GetSortedList();
                //}
                return new PagedList<Entities.Produit>(query, pagingParams.PageNumber, pagingParams.PageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Entities.Produit GetProduit(int id)
        {
            try
            {
                var produit = DbContext.Produit.Where(x => x.Id == id)
                                        .Include(x => x.PrixParFournisseur).ThenInclude(x => x.fournisseur)
                                       .FirstOrDefault();
                return produit;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async Task<bool> IProduitRepository.deletePrixParFournisseur(List<ProduitFournisseur> prixParFournisseurs)
        {
            DbContext.RemoveRange(prixParFournisseurs);
            return await DbContext.SaveChangesAsync() > 0;
        }


        async Task<bool> IProduitRepository.updateFicheTehcnique(int id, List<ficheTechniqueModal> ficheTechniques)
        {
            try
            {
                var produitInDb = DbContext.Produit.SingleOrDefault(x => x.Id == id);

                #region delete files
                //recuperer l'article a partir du base de donnée
                List<ficheTechniqueModal> ficheTechniquesInDB = JsonConvert.DeserializeObject<List<ficheTechniqueModal>>(produitInDb.FichesTechniques);
                var namesofOldFilesInDb = ficheTechniquesInDB.SelectMany(FT => FT.pieceJointes).Select(PJ => PJ.name).ToList();
                var namesOfNewFiles = ficheTechniques.SelectMany(FT => FT.pieceJointes).Select(PJ => PJ.name).ToList();
                var namesOfDeletedFiles = namesofOldFilesInDb.Except(namesOfNewFiles).ToList();
                //delete files
                fileManager.DeleteFiles(namesOfDeletedFiles);
                #endregion delete files

                #region add new files
                var pieceJointes = ficheTechniques.SelectMany(FT => FT.pieceJointes).Where(x => x.file != "").ToList();
                foreach (FTPieceJointes pieceJointe in pieceJointes)
                {
                    fileManager.Save(pieceJointe.file, pieceJointe.name);
                }
                #endregion

                produitInDb.FichesTechniques = JsonConvert.SerializeObject(ficheTechniques);
                DbContext.Update(produitInDb);
                return await DbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
