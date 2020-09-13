using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Departement;
using Serilog;

namespace Projet__de_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartementsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IDepartmentRepository departementRepository;

        public DepartementsController(ProjetBaseContext context)
        {
            _context = context;
            departementRepository = new DepartementRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                var IdPays = filterModel.SearchQuery.ToLower() != "" ? Int32.Parse(filterModel.SearchQuery.ToLower()) : 0;
                return
                    Ok(departementRepository.Filter(
                        filter: x => (IdPays != 0 ? x.IdPays.Value == IdPays : true),
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
                var depart = departementRepository.GetById(id);
                if (depart == null)
                {
                    return NotFound();
                }
                
                return Ok(depart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}