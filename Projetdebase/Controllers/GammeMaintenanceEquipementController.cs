using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.GammeMaintenanceEquipement;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GammeMaintenanceEquipementController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IGammeMaintenanceEquipementRepository gammeMaintenanceEquipementRepository;
        public GammeMaintenanceEquipementController(ProjetBaseContext context)
        {
            _context = context;
            gammeMaintenanceEquipementRepository = new GammeMaintenanceEquipementRepository(_context);

        }

        [HttpPost]
        //[Route("All")]
        public async Task<IActionResult> index([FromBody] GammeMaintenanceEquipementModel filterModel)
        {
            try
            {
                return Ok(gammeMaintenanceEquipementRepository.Filter(filter: x => (
                x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower())

                ),
                pagingParams: filterModel.PagingParams,
                sortingParams: filterModel.SortingParams
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

                var gammeMaintenanceEquipement = gammeMaintenanceEquipementRepository.GetById(id);
                if (gammeMaintenanceEquipement == null)
                {
                    return NotFound();
                }
                return Ok(gammeMaintenanceEquipement);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        // POST: api/GammeMaintenanceEquipement/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GammeMaintenanceEquipement gammeMaintenanceEquipement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                gammeMaintenanceEquipementRepository.Create(gammeMaintenanceEquipement);
                await _context.SaveChangesAsync();
                return Ok(gammeMaintenanceEquipement);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody]  GammeMaintenanceEquipement gammeMaintenanceEquipemene)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                gammeMaintenanceEquipementRepository.Update(gammeMaintenanceEquipemene);
                await _context.SaveChangesAsync();
                return Ok(gammeMaintenanceEquipemene);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
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
                var gammeMaintenanceEquipement = gammeMaintenanceEquipementRepository.GetById(id);

                if (gammeMaintenanceEquipement == null) {
                    return NotFound();
                }

                gammeMaintenanceEquipementRepository.Delete(gammeMaintenanceEquipement);
                _context.SaveChanges();
                return Ok(gammeMaintenanceEquipement);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpGet]
        [Route("CheckUniqueNom/{nom}")]
        public async Task<IActionResult> CheckUniqueNom(string nom)
        {
            try
            {
                return Ok(gammeMaintenanceEquipementRepository.CheckUniqueNom(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


    }
}
