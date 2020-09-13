using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.Attributes;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Paiement;
using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class PaiementsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IPaiementRepository paiementRepository;

        public PaiementsController(ProjetBaseContext context)
        {
            _context = context;
            paiementRepository = new PaiementRepository(context);
        }

        // GET: api/Paiements/All
        [HttpPost]
        [Route("All")]
        public async Task<IActionResult> Get([FromBody] PaiementModel filterModel)
        {
            try
            {
                var list = paiementRepository.Filter(
                            filter: x => (
                                ((x.Description ?? "").ToLower().Contains(filterModel.SearchQuery.ToLower())) &&
                                (filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DatePaiement.Date) &&
                                (filterModel.DateFin == null ? true : filterModel.DateFin >= x.DatePaiement.Date) &&
                                (filterModel.IdCompte.HasValue ? x.IdCaisse == filterModel.IdCompte : true) 
                            ),
                            pagingParams: filterModel.PagingParams,
                            sortingParams: filterModel.SortingParams,
                            include: "ParametrageCompte"
                        );
                var total = await paiementRepository.TotalPaiement(filterModel);
                return Ok(new {
                    Total = total,
                    Data = list
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // POST: api/Paiements/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Paiement paiement)
        {
            try
            {
                if (paiement.IdDepense == null)
                {
                    if (!await paiementRepository.CheckValidePaiementAjout(paiement.FacturePaiements.ToList()))
                    {
                        return Ok(false);
                    }
                }
                if (paiement.IdDepense != null)
                {
                    if (!await paiementRepository.CheckValidePaiementDepenseAjout(paiement))
                    {
                        return Ok(false);
                    }
                }
              

                if (paiement.IdDepense != null)
                {
                    var paiementRes = await paiementRepository.SavePaiementDepense(paiement);
                    return Ok(paiementRes);
                }
                else
                {
                    var  paiementRes = await paiementRepository.Save(paiement);
                  
                        var idFacture = paiement.FacturePaiements.FirstOrDefault().IdFacture; ;
                        var chantier = _context.Factures.Include(x => x.Chantier).SingleOrDefault(x => x.Id == idFacture).Chantier;
                    if(chantier != null)
                    {
                        paiementRepository.changeStatutToTermine(chantier.Id);

                    }

                    return Ok(paiementRes);
                }

           

               
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }

        }

      
        // GET: api/Paiements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var facture = await paiementRepository.GetPaiement(id);

                if (facture == null)
                {
                    return NotFound();
                }

                return Ok(facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // PUT: api/Paiements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Paiement paiement)
        {
            try
            {
                if(paiement.IdDepense == null)
                {
                    if (!await paiementRepository.CheckValidePaimentEdit(paiement.FacturePaiements.ToList()))
                    {
                        return Ok(false);
                    }
                }
               if (paiement.IdDepense != null)
                {
                    if (!await paiementRepository.CheckValidePaimentDepenseEdit(paiement))
                    {
                        return Ok(false);
                    }
                }


                        var paiementInDB = paiementRepository.GetById(paiement.Id);
                //var champsModify = paiementRepository.Historique(paiement, paiementInDB);

                //if (champsModify.Count() > 0)
                //{
                //    champsModify = paiementRepository.Historique(paiement, paiementInDB);
                //    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(paiementInDB.Historique);
                //    var currentUser = EntityExtensions.GetCurrentUser(_context);
                //    hitoriques.Add(new HistoriqueModel()
                //    {
                //        date = DateTime.Now,
                //        action = (int)ActionHistorique.Updated,
                //        IdUser = currentUser.Id,
                //        champs = champsModify
                //    });
                //    paiement.Historique = JsonConvert.SerializeObject(hitoriques);
                //}

                var champsModify = EntityExtensions.GetModification(paiementInDB, paiement);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = paiementRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(paiementInDB.Historique);
                    var userManager = new UserManager(_context);
                    var ccurrentUser = userManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = ccurrentUser.Id,
                        champs = champsModify
                    });
                    paiement.Historique = JsonConvert.SerializeObject(hitoriques);
                }
                if (paiement.IdDepense != null)
                {
                    await paiementRepository.UpdatePaiementDepense(paiement);
                }
                else
                {
                    await paiementRepository.UpdatePaiement(paiement);
                }
               
                return Ok(paiement);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // DELETE: api/Paiements/5
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var paiement = paiementRepository.GetById(x => x.Id == id, include: "FacturePaiements,Avoir");
             

                if (paiement == null)
                {
                    return NotFound();
                }

                if (paiement.IdDepense == null)
                {
                    paiementRepository.DeletePaiement(paiement);
                }
                else
                {
                    paiementRepository.DeletePaiementDepense(paiement);
                }


                return Ok(paiement);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [Route("MovementCompteACompte")]
        [HttpPost]
        public async Task<IActionResult> MovementCompteACompte([FromBody] PaiementMovementCompteACompteModel paiementMovementCompte)
        {
            try
            {
                var res = await paiementRepository.MovementCompteACompte(paiementMovementCompte);

                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}