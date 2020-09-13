using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Pays;
using Serilog;

namespace Projet__de_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaysController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IPaysRepository paysRepository;

        public PaysController(ProjetBaseContext context)
        {
            _context = context;
            paysRepository = new PaysRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(paysRepository.Filter(
                        filter: x => (x.NomFrFr.ToLower().Contains(filterModel.SearchQuery.ToLower())),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
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
                var pays = paysRepository.GetById(id);
                if (pays == null)
                {
                    return NotFound();
                }
                
                return Ok(pays);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}