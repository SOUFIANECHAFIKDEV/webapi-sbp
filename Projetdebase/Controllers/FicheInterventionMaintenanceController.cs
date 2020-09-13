using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Repositories.FicheInterventionMaintenance;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FicheInterventionMaintenanceController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IFicheInterventionMaintenanceRepository ficheInterventionMaintenanceRepository;

        public FicheInterventionMaintenanceController(ProjetBaseContext context)
        {
            _context = context;
            ficheInterventionMaintenanceRepository = new FicheInterventionMaintenanceRepository(_context);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var ficheIntervention = ficheInterventionMaintenanceRepository.GetFicheInterventionMaintenance(id);

                if (ficheIntervention == null)
                {
                    return NotFound();
                }

                return Ok(ficheIntervention);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FicheInterventionMaintenance ficheInterventionMaintenance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ficheInterventionMaintenance.DateCreation = DateTime.Now;

                ficheInterventionMaintenanceRepository.Create(ficheInterventionMaintenance);
                await _context.SaveChangesAsync();

                return Ok(ficheInterventionMaintenance);
            }
            catch (Exception ex)
            {

                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("CheckUniqueReference/{reference}")]
        public async Task<IActionResult> CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(ficheInterventionMaintenanceRepository.CheckUniqueReference(reference));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}
