using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Outils.Pdf_Devis;
using ProjetBase.Businnes.Repositories.BonCommandeFournisseur;
using ProjetBase.Businnes.Repositories.Devis;
using ProjetBase.Businnes.Repositories.Parametrage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class DevisController : ControllerBase
    {
        
        private readonly ProjetBaseContext _context;
        private readonly IDevisRepository devisRepository;

        private readonly IParametrageRepository parametrageRepository;
        private readonly IBonCommandeFournisseur bonCommandeFournisseurRepository;
        private IPdfdevis PdfDevis;

        public DevisController(ProjetBaseContext context)
        {
            _context = context;
            devisRepository = new DevisRepository(_context);
            parametrageRepository = new ParametrageRepository(_context);
            bonCommandeFournisseurRepository = new BonCommandeFournisseurtRepository(_context);
        }
        

        [HttpPost]
        public IActionResult Index([FromBody] DevisModelFilter filterModel)
        {
            try
            {
                return Ok(devisRepository.Filter(
                  filter: x => (
                        (x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Objet.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                        && (filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true)
                             && (filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Status) : true)
                  ),

                  pagingParams: filterModel.PagingParams,
                  sortingParams: filterModel.SortingParams,
                  include: "Chantier,Facture,BonCommandeFournisseur"
                  ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var devis = devisRepository.GetDevis(id);

                if (devis == null)
                {
                    return NotFound();
                }

                return Ok(devis);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Devis devis)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                devis.DateCreation = DateTime.Now;
                devisRepository.Create(devis);
                await _context.SaveChangesAsync();
                return Ok(devis);
            }
            catch (Exception ex)
            {

                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Devis devis)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var devisInDB = devisRepository.GetById(id);

                if (devisInDB == null)
                {
                    return NotFound();
                }
                devis.DateCreation = devisInDB.DateCreation;
                var champsModify = EntityExtensions.GetModification(devisInDB, devis);

                if (champsModify.Count > 0)
                {
                    champsModify = devisRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(devisInDB.Historique);

                    if (champsModify.Count > 0)
                    {
                        var userManager = new UserManager(_context);
                        var currentUser = userManager.GetCurrentUser();
                        hitoriques.Add(new HistoriqueModel()
                        {
                            date = DateTime.Now,
                            action = (int)ActionHistorique.Updated,
                            IdUser = currentUser.Id,

                            champs = champsModify
                        });

                    }



                    devis.Historique = JsonConvert.SerializeObject(hitoriques);
                }

                bool result = await devisRepository.changerStatutChantier(devis);
                if (!result)
                {
                    return BadRequest();
                }

                devis.Id = id;
                devisRepository.Update(devis);
                await _context.SaveChangesAsync();
                return Ok(devis);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                //var devis = devisRepository.GetById(id);

                //if (devis == null)
                //{
                //    return NotFound();
                //}

                //devisRepository.Delete(devis);

                //_context.SaveChanges();

                //return Ok(true);
                var devis = devisRepository.GetById(id);
                if (devis == null)
                {
                    return NotFound();
                }
                var result = devisRepository.supprimerDevis(id);

                _context.SaveChanges();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("CheckUniqueReference/{reference}")]
        public IActionResult CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(devisRepository.CheckUniqueReference(reference));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("GeneratePDF/{id}")]
        public IActionResult GeneratePdf(int id)
        {
            try
            {
                var devis = devisRepository.GetDevis(id);
                var param = parametrageRepository.GetParametrageDocument();
                PdfDevis = new Pdfdevis(devis, param);
                return Ok(PdfDevis.GenerateDocument());

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("sendEmail/{idDevis}")]
        public async Task<IActionResult> sendEmail(int idDevis, [FromBody] InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {

                return Ok(await devisRepository.SendEmail(idDevis, MailParams));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("changeStatut")]
        public async Task<IActionResult> changeStatut(ChangeStatutBodyRequestDevis body)
        {
            try
            {
                Devis devisInDb = devisRepository.GetDevis(body.iddevis);
                devisInDb.Status = body.statutDevis;
                bool result = await devisRepository.changerStatutChantier(devisInDb);
                if (!result)
                {
                    return BadRequest();
                }
                devisRepository.Update(devisInDb);
                return Ok(await _context.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        private List<int> selectDistinctIdsFournisseur(List<PrestationsModule> prestations)
        {
            try
            {
                var ids = new List<int>();
               // var vv = prestations.ToList().Where(P => P.data.lotProduits != null).Select(P => P.data.lotProduits).Distinct();
                //var prestations.
                var xprestations = prestations.Where(x=>x.type == 1).ToList();
                foreach (PrestationsModule cc in xprestations)
                {
                    var xxx = cc.data.prixParFournisseur.Where(x => x.@default == 1).FirstOrDefault();
                    if (xxx != null)
                    {
                        ids.Add(xxx.idFournisseur);
                    }
                }

                var f = ids.ToList().Distinct().ToList();
                return f;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private List<int> selectDistinctIdsFournisseurInLot(List<Data> prestations)
        {
            try
            {
                var ids = new List<int>();
                var xprestations = prestations.ToList();
                foreach (Data cc in xprestations)
                {
                    var xxx = cc.prixParFournisseur.Where(x => x.@default == 1).FirstOrDefault();
                    if (xxx != null)
                    {
                        ids.Add(xxx.idFournisseur);
                    }
                }

                var f = ids.ToList().Distinct().ToList();
                return f;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("generateBC")]
        public async Task<IActionResult> generateBC(Devis devis)
        {
            try
            {
                
                //recupere les prestaions du devis
                List <PrestationsModule> prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(devis.Prestation);
                //pour generer le reference
                var parametrage = parametrageRepository.GetById(x => x.Type == (int)TypeParametrage.numerotaion);
                // liste des bon de commande d un devis
                var ListBC = new List<BonCommandeFournisseur>();
                //
                var Listfournisseur = new List<int>();
                for (int i = 0; i < prestations.Count; i++)
                {
                    if (prestations[i].type == (int)typePrestation.Lot || prestations[i].data.lotProduits != null)
                    {
                        List<DataLotProduit> lotProduits = prestations[i].data.lotProduits;
                        //selectionner la liste produit qui est dans lot                       
                        List<Data> ccdataLotProduit = lotProduits.Select(P => P.idProduitNavigation).ToList();

                        var idff = selectDistinctIdsFournisseurInLot(ccdataLotProduit);
                        Listfournisseur.AddRange(idff);
                    }
                    if (prestations[i].type == (int)typePrestation.Produit)
                    {
                        var ids = selectDistinctIdsFournisseur(prestations);
                        Listfournisseur.AddRange(ids);
                    }
                }  

                var numberGroups = Listfournisseur.Where(e => e != 0).GroupBy(Key => Key);


                foreach (var grpfournisseur in numberGroups)
                {
                    int idd = grpfournisseur.Key;
                    //prestation lot
                    var prestationsInLot = prestations.Where(P => P.data.lotProduits != null).ToList();

                    List<Data> ListPrestationsFournisseurInLot= new List<Data>();
                    foreach (var p in prestationsInLot)
                    {
                        List<Data> fournisseurPrestationsInLot = new List<Data>();
                        Data DataPrestations = p.data;
                        List<DataLotProduit> dataLotProduit = DataPrestations.lotProduits;
                        fournisseurPrestationsInLot = dataLotProduit.Where(e => e.idProduitNavigation.prixParFournisseur.Where(x => x.idFournisseur == idd && x.@default == 1).FirstOrDefault() != null).Select(e => e.idProduitNavigation).ToList();

                        ListPrestationsFournisseurInLot.AddRange(fournisseurPrestationsInLot);

                    }
                    //prestation fournisseur
                    var fournisseurPrestations = ListPrestationsFournisseurInLot.Concat(
                        prestations.Where(P => P.type == 1 &&
                        P.data.prixParFournisseur.Where(x => x.idFournisseur == idd && x.@default == 1).FirstOrDefault() != null).Select(e => e.data).ToList()).ToList();


                    var type = TypeNumerotation.boncommande_fournisseur;
                    List<Parametrage> contenu = JsonConvert.DeserializeObject<List<Parametrage>>(parametrage.Contenu);
                    Parametrage numerotationInfos = contenu.Where(x => x.Type == (int)type).FirstOrDefault();
                    var refd = await parametrageRepository.GenerateParameter(numerotationInfos);
                    var bonCommandes = devisRepository.createBCObjet(devis, idd, fournisseurPrestations);
                    bonCommandes.Reference = refd;

                    ListBC.Add(bonCommandes);
                    parametrage = await parametrageRepository.IncrementReference(parametrage, type);
                }

                devis.BonCommandeFournisseur = ListBC;
                devis.Status = StatutDevis.Acceptee;
                devisRepository.Update(devis);
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
                return Ok(devis);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}
