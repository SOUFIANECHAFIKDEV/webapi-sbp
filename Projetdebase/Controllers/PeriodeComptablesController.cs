using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.PeriodeComptable;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PeriodeComptablesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IPeriodeComptablesRepository periodeComptablesRepository;

        public PeriodeComptablesController(ProjetBaseContext context)
        {
            _context = context;
            periodeComptablesRepository = new PeriodeComptablesRepository(_context);
        }


        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                var currentUser = EntityExtensions.GetCurrentUser(_context);
                return
                    Ok(periodeComptablesRepository.Filter(
                        filter: x => (!x.DateCloture.HasValue),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
                        )
                    );
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }

        }

        //Post: api/PeriodeComptables/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PeriodeComptable periodeComptable)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var currentUser = EntityExtensions.GetCurrentUser(_context);
                periodeComptable.Id = currentUser.Id;
                periodeComptablesRepository.Create(periodeComptable);
                await _context.SaveChangesAsync();
                return Ok(periodeComptable);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        //put: API/PeriodeComptables/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PeriodeComptable periodeComptable)
        {
            try
            {
                var periodeInDb = periodeComptablesRepository.GetById(periodeComptable.Id);

                if (periodeInDb == null)
                {
                    return NotFound();
                }

                periodeComptablesRepository.Update(periodeComptable);
                await _context.SaveChangesAsync();
                return Ok(periodeComptable);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        //GET/ api/PriodeComptable/CloturePeriode/5
        [HttpGet("CloturePeriode/{id}")]
        public async Task<IActionResult> CloturePeriode([FromRoute] int id)
        {
            try
            {
                await periodeComptablesRepository.CloturePeriode(id);
                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
        
    }
}
