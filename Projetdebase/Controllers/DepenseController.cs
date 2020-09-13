using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Depense;
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
    public class DepenseController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IDepenseRepository depenseRepository;

        public DepenseController(ProjetBaseContext context)
        {
            _context = context;
            depenseRepository = new DepenseRepository(_context);

        }

        [HttpPost]
        public IActionResult Index([FromBody] DepenseFilterModel filterModel)
        {
            try
            {
                return Ok(depenseRepository.Filter(
                   filter: x => (
                         (x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                         && (filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true)
                              && (filterModel.IdFournisseur.HasValue ? (x.IdFournisseur == filterModel.IdFournisseur) : true)
                              && (filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Status) : true)
                   ),

                   pagingParams: filterModel.PagingParams,
                   sortingParams: filterModel.SortingParams,
                   //include: ""
                   include: "Chantier,Fournisseur,DepenseBonCommandeFournisseurs"
                   ));

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
              var depense = depenseRepository.GetDepense(id);
            
             
                if (depense == null)
                {
                    return NotFound();
                }

                return Ok(depense);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DepensePostModel depensePostModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //depense.DateCreation = DateTime.Now;
                depenseRepository.Create(depensePostModel.Depense);
                await _context.SaveChangesAsync();
                List <DepenseBonCommandeFournisseur> ListdepenseBonCommandeFournisseur = ListdepenseBonCommandeFournisseur = new List<DepenseBonCommandeFournisseur>();

                if (depensePostModel.BonCommandeFournisseurIds.Count() > 0)
                {
                    foreach (var idBonCommandFournisseur in depensePostModel.BonCommandeFournisseurIds)
                    {
                        var bonCommandeFournisseur = _context.BonCommandeFournisseur.Where(x => x.Id == idBonCommandFournisseur).FirstOrDefault();

                       var DepenseBonCommandeFournisseurs =   depenseRepository.DepenseBonCommandeFournisseur(idBonCommandFournisseur, depensePostModel.Depense.Id, bonCommandeFournisseur
                           );
                        ListdepenseBonCommandeFournisseur.Add(DepenseBonCommandeFournisseurs);
                    }
                }
               
                depensePostModel.Depense.DepenseBonCommandeFournisseurs = ListdepenseBonCommandeFournisseur;
                _context.Depense.Update(depensePostModel.Depense);
                await _context.SaveChangesAsync();
                return Ok(depensePostModel.Depense);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Depense depense)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                depense.Id = id;
                var DepenseInDB = depenseRepository.GetById(depense.Id);

                if (DepenseInDB == null)
                {
                    return NotFound();
                }

                // Get differte between object send and stored object
                //  var champsModify = EntityExtensions.GetModification(DepenseInDB, depense);
                var champsModify = EntityExtensions.GetModification(DepenseInDB, depense);

                // Add Historique
                //if (champsModify.Count > 0)
                //{
                //    champsModify = depenseRepository.ChangeIdToNameHistorique(champsModify);
                //    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(DepenseInDB.Historique);
                //    var userManager = new UserManager(_context);
                //    var currentUser = userManager.GetCurrentUser();
                //    hitoriques.Add(new HistoriqueModel()
                //    {
                //        date = DateTime.Now,
                //        action = (int)ActionHistorique.Updated,
                //        IdUser = currentUser.Id,
                //        champs = champsModify
                //    });
                //    depense.Historique = JsonConvert.SerializeObject(hitoriques);
                //}
                if (DepenseInDB.Historique != null)
                {
                    if (champsModify.Count > 0)
                    {
                        champsModify = depenseRepository.ChangeIdToNameHistorique(champsModify);
                        var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(DepenseInDB.Historique);
                        var userManager = new UserManager(_context);
                        var currentUser = userManager.GetCurrentUser();
                        hitoriques.Add(new HistoriqueModel()
                        {
                            date = DateTime.Now,
                            action = (int)ActionHistorique.Updated,
                            IdUser = currentUser.Id,
                            champs = champsModify
                        });
                        depense.Historique = JsonConvert.SerializeObject(hitoriques);
                    }
                }

                depenseRepository.Update(depense);


                await _context.SaveChangesAsync();
                return Ok(depense);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var depense = depenseRepository.GetById(id);

                if (depense == null)
                {
                    return NotFound();
                }

                _context.Remove(depense);

                var result = await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> SaveMemos([FromRoute] int id, [FromBody] string memos)
        {

        
            try
            {
                return Ok(await depenseRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
