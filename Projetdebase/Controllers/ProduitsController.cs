using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Repositories.Produit;
using Serilog;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using InoAuthentification.UserManagers;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using ProjetBase.Businnes.Repositories.Lots;
using static ProjetBase.Businnes.Models.FicheTechniqueModal;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitsController : Controller
    {
        private readonly ProjetBaseContext _context;
        private readonly IProduitRepository produitRepository;
        private readonly ILotsRepository lotsRepository;

        public ProduitsController(ProjetBaseContext context)
        {
            _context = context;
            produitRepository = new ProduitRepository(_context);
            lotsRepository = new LotsRepository(_context);
        }



        public IEnumerable<int> DeserializeObject(string Produits)
        {
            IEnumerable<int> produitsIds = JsonConvert.DeserializeObject<IEnumerable<int>>(Produits);
            return produitsIds;
        }

        //récupérer la liste de produits
        [HttpPost]
        public IActionResult Index([FromBody] ProduitsFilterModel filterModel)
        {
            try
            {
                return Ok(
                   produitRepository.Filter(
                       IdFournisseur: filterModel.IdFournisseur,
                       pagingParams: filterModel.PagingParams,
                        filter: x => (
                        (filterModel.Labels.Count() == 0 ? true : filterModel.Labels.Any(item => x.Labels.ToUpper().Contains("\"" + item.ToUpper() + "\"")))
                         && (String.IsNullOrEmpty(filterModel.Categorie) ? true : (x.Categorie.ToUpper() == filterModel.Categorie.ToUpper()))

                        &&
                        (x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()))),
                       sortingParams: filterModel.SortingParams

                   ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer un produit par son id
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                //  var produit = produitRepository.GetById(filter: x => x.Id == id, include: "PrixParFournisseur");

                var produit = produitRepository.GetProduit(id);

                if (produit == null)
                {
                    return NotFound();
                }

                return Ok(produit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //ajouter un nouveau produit
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produit produit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                produitRepository.Create(produit);
                await _context.SaveChangesAsync();
                return Ok(produit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //modifier les information d'une produit
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Produit produit)
        {
            try
            {
                #region vérifier l'existence du produit qui on veut le modifier
                var produitInDB = _context.Produit.Include(x => x.PrixParFournisseur).SingleOrDefault(x => x.Id == produit.Id);
                if (produitInDB == null) return NotFound();
                await produitRepository.deletePrixParFournisseur(produitInDB.PrixParFournisseur);
                #endregion

                #region Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(produitInDB, produit);
                var currentUser = EntityExtensions.GetCurrentUser(_context);
                #endregion

                #region Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = produitRepository.ChangeIdToNameHistorique(champsModify);
                    if (champsModify.Count() > 0)
                    {
                        var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(produitInDB.Historique);
                        hitoriques.Add(new HistoriqueModel()
                        {
                            date = DateTime.Now,
                            action = (int)ActionHistorique.Updated,
                            IdUser = currentUser.Id,
                            champs = champsModify
                        });
                        produit.Historique = JsonConvert.SerializeObject(hitoriques);
                    }

                }
                #endregion

                #region affecter les champs modifier
                produitInDB.Reference = produit.Reference;
                produitInDB.Nom = produit.Nom;
                produitInDB.Description = produit.Description;
                produitInDB.Designation = produit.Designation;
                produitInDB.Categorie = produit.Categorie;
                produitInDB.Historique = produit.Historique;
                produitInDB.FichesTechniques = produit.FichesTechniques;
                produitInDB.Labels = produit.Labels;
                produitInDB.Lot = produit.Lot;
                produitInDB.LotProduits = produit.LotProduits;
                produitInDB.Tva = produit.Tva;
                produitInDB.Unite = produit.Unite;
                produitInDB.PrixHt = produit.PrixHt;
                produitInDB.Nomber_heure = produit.Nomber_heure;
                produitInDB.Cout_horaire = produit.Cout_horaire;
                produitInDB.Cout_materiel = produit.Cout_materiel;
                produitInDB.Cout_vente = produit.Cout_vente;
                produitInDB.PrixParFournisseur = produit.PrixParFournisseur;
                #endregion


                _context.Update(produitInDB);

                await _context.SaveChangesAsync();

                return Ok(produit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //supprimer un produit
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var produit = produitRepository.GetById(id);

                if (produit == null)
                {
                    return NotFound();
                }

                var result = produitRepository.supprimerProduit(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        //verifier que la reference de produit est unique
        [HttpGet]
        [Route("CheckUniqueReference/{reference}")]
        public IActionResult CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(produitRepository.CheckUniqueReference(reference));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //sauvegarder les mémos de produit
        [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> saveMemos([FromRoute] int id, [FromBody] string memos)
        {
            try
            {
                return Ok(await produitRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer la liste des tva
        [HttpGet]
        [Route("ListeTva")]
        public IActionResult getListeTva()
        {
            try
            {
                return Ok(produitRepository.getListeTva());
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer la tva par id
        [HttpGet]
        [Route("getTvaById/{id}")]
        public IActionResult getTvaById([FromRoute] int id)
        {
            try
            {
                return Ok(produitRepository.getTvaById(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer l'Unite de produit par id
        [HttpGet]
        [Route("ListeUnite")]
        public IActionResult ListeUnite()
        {
            try
            {
                return Ok(produitRepository.getListeUnite());
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer l'Unite de produit par id
        [HttpGet]
        [Route("getUniteById/{id}")]
        public IActionResult getUniteById([FromRoute] int id)
        {
            try
            {
                return Ok(produitRepository.getUniteById(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer la Categorie de produit par id
        [HttpGet]
        [Route("ListeCategorie")]
        public IActionResult getListeCategorie()
        {
            try
            {
                return Ok(produitRepository.getListeCategorie());
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //récupérer la Categorie de produit par id
        [HttpGet]
        [Route("getCategorieById/{id}")]
        public IActionResult getCategorieById([FromRoute] int id)
        {
            try
            {
                return Ok(produitRepository.getCategorieById(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("getLotProduits")]
        public IActionResult getLotProduits([FromBody] ProduitsLotsFilterModel filterModel)
        {
            try
            {
                var result = produitRepository.Filter(
                        filter: x => ((filterModel.Labels.Any(item => x.Labels.Contains(item))) &&
                        (x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                        && (filterModel.produits.Contains(x.Id))),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams
                        );

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateFicheTehcnique/{id}")]
        [HttpPost]
        public async Task<IActionResult> updateFicheTehcnique([FromRoute] int id, [FromBody] List<ficheTechniqueModal> files)
        {
            try
            {
                var result = await produitRepository.updateFicheTehcnique(id, files);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}