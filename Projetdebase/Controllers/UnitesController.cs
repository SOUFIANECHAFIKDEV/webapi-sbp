using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Unites;
using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly UniteRepository uniteRepository;

        public UnitesController(ProjetBaseContext context)
        {
            _context = context;
            uniteRepository = new UniteRepository(_context);

        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(uniteRepository.Filter(
                        filter: x => (x.NomComplet.Contains(filterModel.SearchQuery) || x.Abreviation.Contains(filterModel.SearchQuery)),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Unite unite)
        {
            try
            {
                uniteRepository.Create(unite);
                await _context.SaveChangesAsync();
                return Ok(unite);
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
                var unite = uniteRepository.GetById(id);
                if (unite == null)
                    return NotFound();

                uniteRepository.Delete(unite);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Unite unite)
        {
            try
            {
                uniteRepository.Update(unite);
                await _context.SaveChangesAsync();
                return Ok(unite);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}