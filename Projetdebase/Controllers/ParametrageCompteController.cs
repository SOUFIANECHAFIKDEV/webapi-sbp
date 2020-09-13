using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.ParametrageCompte;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;

using Microsoft.EntityFrameworkCore;
using InoAuthentification.Attributes;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class ParametrageCompteController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IParametrageCompteRepository parametrageCompteRepository;

        public ParametrageCompteController(ProjetBaseContext context)
        {
            _context = context;
            parametrageCompteRepository = new ParametrageCompteRepository(_context);
        }

        //GET : api/CompteBancaire
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(parametrageCompteRepository.Filter(
                        filter: x => (x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.code_comptable.ToLower().Contains(filterModel.SearchQuery.ToLower())),
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


        //GET: api/ParametrageCompte/
        [HttpGet("{id}")]
        public IActionResult GetParametrageCompte([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parametrageCompte = parametrageCompteRepository.GetById(x=> x.Id == id);

            if (parametrageCompte == null)
            {
                return NotFound();
            }

            return Ok(parametrageCompte);
        }

        // PUT: api/ParametrageCompte/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParametrageCompte([FromRoute] int id, [FromBody] ParametrageCompte parametrageCompte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parametrageCompte.Id)
            {
                return BadRequest();
            }

            try
            {   
                parametrageCompteRepository.Update(parametrageCompte);
                await _context.SaveChangesAsync();
                return Ok(parametrageCompte);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametrageCompteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ParametrageCompte parametrageComptes)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                _context.ParametrageCompte.Add(parametrageComptes);
                await _context.SaveChangesAsync();

                return Ok(parametrageComptes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        // DELETE: api/ParametrageCompte/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParametrageCompte([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parametrageCompte = parametrageCompteRepository.GetById(x => x.Id == id);
            if (parametrageCompte == null)
            {
                return NotFound();
            }
            
            parametrageCompteRepository.Delete(parametrageCompte);
            await _context.SaveChangesAsync();

            return Ok(parametrageCompte);
        }

        private bool ParametrageCompteExists(int id)
        {
            return _context.ParametrageCompte.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("CheckUniqueNom/{nom}")]
        public async Task<IActionResult> CheckUniqueNom(string nom)
        {
            try
            {
                return Ok(parametrageCompteRepository.CheckUniqueNom(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }



    }
}
