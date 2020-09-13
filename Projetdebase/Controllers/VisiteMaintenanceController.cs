using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.VisiteMaintenance;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisiteMaintenanceController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IVisiteMaintenanceRepository visiteMaintenanceRepository;
        public VisiteMaintenanceController(ProjetBaseContext context)
        {
            _context = context;
            visiteMaintenanceRepository = new VisiteMaintenanceRepository(_context);

        }

        // GET: api/VisiteMaintenance/All
        [HttpPost]
        [HttpPost]
        [Route("All")]
        public async Task<IActionResult> Get([FromBody] VisiteMaintenanceModel filterModel)
        {
           


                //return Ok(visiteMaintenanceRepository.Filter(
                //        pagingParams: filterModel.PagingParams,
                //        filter: x => (
                //           (filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Statut) : true) &&
                //           (filterModel.Annee.HasValue ? (filterModel.Annee == x.Annee) : true) &&
                //            (filterModel.Mois.HasValue ? (filterModel.Mois == x.Mois) : true) &&
                //           ((!filterModel.IdClient.HasValue) ? true : x.ContratEntretien.IdClient == filterModel.IdClient) 
                //       ),
                //        sortingParams: filterModel.SortingParams,
                //        include: "ContratEntretien,FicheInterventionMaintenance"
                //    ));
                try
                {
                    return Ok(
                        visiteMaintenanceRepository.Filter(
                            pagingParams: filterModel.PagingParams,
                            filter: x => (
                                ((filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Statut) : true))
                                  && (filterModel.Annee.HasValue ? (filterModel.Annee == x.Annee) : true)
                                  && (filterModel.Mois.HasValue ? (filterModel.Mois == x.Mois) : true)
                                  && ((!filterModel.IdClient.HasValue) ? true : x.ContratEntretien.IdClient == filterModel.IdClient)
                                  ),
                            sortingParams: filterModel.SortingParams
                        ));
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // GET: api/VisiteMaintenance/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var VisiteMaintenance =  visiteMaintenanceRepository.GetVisiteMaintenance(id);

                if (VisiteMaintenance == null)
                {
                    return NotFound();
                }

                return Ok(VisiteMaintenance);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("changeStatut")]
        public async Task<IActionResult> changeStatut(ChangeStatutBodyRequestVisiteMaintenance body)
        {
            try
            {
                VisiteMaintenance visiteMaintenanceInDb = visiteMaintenanceRepository.GetById(body.idVisiteMaintenance);
                visiteMaintenanceInDb.Statut = (int)body.statutVisiteMaintenance;
             
                visiteMaintenanceRepository.Update(visiteMaintenanceInDb);
             
                await _context.SaveChangesAsync();
                return Ok(visiteMaintenanceInDb);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
