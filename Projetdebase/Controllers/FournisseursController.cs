using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Fournisseur;
using Serilog;
using System;
using ProjetBase.Businnes.Models;
using System.Threading.Tasks;
using ProjetBase.Businnes.Enum;
using System.Collections.Generic;
using Newtonsoft.Json;
using InoAuthentification.UserManagers;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FournisseursController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IFournisseurRepository fournisseurRepository;

        public FournisseursController(ProjetBaseContext context)
        {
            _context = context;
            fournisseurRepository = new FournisseurRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(fournisseurRepository.Filter(
                        filter: x => (x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower())),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams
                  
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var fournisseur = fournisseurRepository.GetById(filter: x => x.Id == id, include: "Pays");
                if (fournisseur == null)
                {
                    return NotFound();
                }
                
                return Ok(fournisseur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Fournisseur fournisseur)
        {
            try
            {
                fournisseurRepository.Create(fournisseur);
                await _context.SaveChangesAsync();
                return Ok(fournisseur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Fournisseur fournisseur)
        {
            try
            {
                // Get object stored in database before modify
                var fournisseurInDB = _context.Fournisseurs.Find(fournisseur.Id);

                // Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(fournisseurInDB, fournisseur);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = fournisseurRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(fournisseurInDB.Historique);
                    var userManager = new UserManager(_context);
                    var currentUser = userManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = currentUser.Id,
                        champs = champsModify
                    });
                    fournisseur.Historique = JsonConvert.SerializeObject(hitoriques);
                }

                // Updated object
                fournisseurRepository.Update(fournisseur);
                await _context.SaveChangesAsync();
                return Ok(fournisseur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var fournisseur = fournisseurRepository.GetById(id);
                if (fournisseur == null)
                {
                    return NotFound();
                }
                var result = fournisseurRepository.supprimerFournisseur(id);
               
                _context.SaveChanges();
                return Ok(result);
               // return Ok(true);
              
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
                return Ok(fournisseurRepository.CheckUniqueReference(reference));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> saveMemos([FromRoute] int id, [FromBody] string memos)
        {
            try
            {
                return Ok(await fournisseurRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}