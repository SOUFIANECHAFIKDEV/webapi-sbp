using Castle.Core.Configuration;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Account;
using ProjetBase.Businnes.Repositories.BonCommandeFournisseur;
//using ProjetBase.Businnes.Repositories.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projet__de_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonCommandeFournisseurController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IBonCommandeFournisseur bonCommandeFournisseurRepository;

        public BonCommandeFournisseurController(ProjetBaseContext context)
        {
            _context = context;
            bonCommandeFournisseurRepository = new ProjetBase.Businnes.Repositories.BonCommandeFournisseur.BonCommandeFournisseurtRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] BonCommandeFournisseurFilterModel filterModel)
        {
            try
            {
                return Ok(bonCommandeFournisseurRepository.Filter(
                        filter: x =>
                        ((filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DateCreation.Date)
                         &&  (filterModel.DateFin == null ? true : filterModel.DateFin >= x.DateExpiration.Date)
                         &&  x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower())
                         && (filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true)
                        && (filterModel.IdFournisseur.HasValue ? (x.IdFournisseur == filterModel.IdFournisseur) : true)
                        && (filterModel.IdClient.HasValue ? x.Chantier.IdClient == filterModel.IdClient : true)
                        && (filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Status) : true)
                        ),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: "Chantier,Fournisseur"
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var bonCommandeFournisseur = bonCommandeFournisseurRepository.GetBonCommandeFournisseur(id);

                if (bonCommandeFournisseur == null)
                {
                    return NotFound();
                }

                return Ok(bonCommandeFournisseur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BonCommandeFournisseur BonCommandeFournisseur)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bonCommandeFournisseurRepository.Create(BonCommandeFournisseur);

                await _context.SaveChangesAsync();

                return Ok(BonCommandeFournisseur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] BonCommandeFournisseur bonCommandeFournisseur)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bonCommandeFournisseur.Id = id;
                var BonCommandeFournisseurInDB = bonCommandeFournisseurRepository.GetBonCommandeFournisseur(bonCommandeFournisseur.Id);

                if (BonCommandeFournisseurInDB == null)
                {
                    return NotFound();
                }

                // Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(BonCommandeFournisseurInDB, bonCommandeFournisseur);

                // Add Historique
                if(BonCommandeFournisseurInDB.Historique != null)
                {
                    if (champsModify.Count > 0)
                    {
                        champsModify = bonCommandeFournisseurRepository.ChangeIdToNameHistorique(champsModify);
                        var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(BonCommandeFournisseurInDB.Historique);
                        var userManager = new UserManager(_context);
                        var currentUser = userManager.GetCurrentUser();
                        hitoriques.Add(new HistoriqueModel()
                        {
                            date = DateTime.Now,
                            action = (int)ActionHistorique.Updated,
                            IdUser = currentUser.Id,
                            champs = champsModify
                        });
                        bonCommandeFournisseur.Historique = JsonConvert.SerializeObject(hitoriques);
                    }
                }


                bonCommandeFournisseurRepository.Update(bonCommandeFournisseur);


                await _context.SaveChangesAsync();
                return Ok(bonCommandeFournisseur);
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

                var client = bonCommandeFournisseurRepository.GetById(id);

                if (client == null)
                {
                    return NotFound();
                }

                _context.Remove(client);

                var result = await _context.SaveChangesAsync();

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
        public async Task<IActionResult> CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(bonCommandeFournisseurRepository.CheckUniqueReference(reference));
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
                return Ok(await bonCommandeFournisseurRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}