using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Ville;
using Serilog;

namespace Projet__de_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IVilleRepository villeRepository;

        public VillesController(ProjetBaseContext context)
        {
            _context = context;
            villeRepository = new VilleRepository(_context);
        }

        [HttpPost]
        public IActionResult Get([FromBody] FilterModel filterModel)
        {
            try
            {
                var IdDepartement = filterModel.SearchQuery != "" ? Int32.Parse(filterModel.SearchQuery) : 0;
                return
                    Ok(villeRepository.Filter(
                        filter: x => (IdDepartement != 0 ? x.IdDepartement.Value == IdDepartement : true),
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

        [Route("Search")]
        [HttpPost]
        public IActionResult Search([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(villeRepository.Filter(
                        filter: x => x.VilleNomReel.Contains(filterModel.SearchQuery),
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
        public IActionResult Get(int id)
        {
            try
            {
                var ville = villeRepository.GetById(id);
                if (ville == null)
                {
                    return NotFound();
                }
                
                return Ok(ville);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}