using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Lots;
using ProjetBase.Businnes.Repositories.Pays;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly ILotsRepository lotsRepository;

        public LotsController(ProjetBaseContext context)
        {
            _context = context;
            lotsRepository = new LotsRepository(_context);
        }


        [HttpPost]
        public async Task<IActionResult> Index([FromBody] FilterModel filterModel)
        {
            try
            {
                return Ok(
                    lotsRepository.Filter(
                        pagingParams: filterModel.PagingParams,
                        filter: x => (
                            x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower())),
                        sortingParams: filterModel.SortingParams
                    ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return NotFound();
            }
        }



        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Lots lots)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                lotsRepository.Create(lots);

                await _context.SaveChangesAsync();

                return Ok(lots);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Lots lots)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clientInDB = lotsRepository.GetById(lots.Id);

                IQueryable<LotProduits> lotProduits  = _context.LotProduits.Where(l => l.IdLot == clientInDB.Id);
                _context.RemoveRange(lotProduits);

                if (clientInDB == null)
                {
                    return NotFound();
                }
                lotsRepository.Update(lots);
                await _context.SaveChangesAsync();
                return Ok(lots);
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

                var lots = lotsRepository.GetById(id);

                if (lots == null)
                {
                    return NotFound();
                }
                var result = lotsRepository.supprimerLots(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("getLotProduitById/{id}")]
        public List<int> getLotProduitById([FromRoute] int id)
        {
                           
                var lotsproduit =  _context.LotProduits.Where(x => x.IdLot == id).Select(y => y.IdProduit).ToList();
             
                return lotsproduit;
              
        }

        [HttpGet]
        [Route("CheckUniqueReference/{nom}")]
        public async Task<IActionResult> CheckUniqueReference(string nom)
        {
            try
            {
                return Ok(lotsRepository.CheckUniqueReference(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}
