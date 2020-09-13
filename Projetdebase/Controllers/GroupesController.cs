using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Groupe;
using Serilog;

namespace SBP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IGroupeRepository groupeRepository;

        public GroupesController(ProjetBaseContext context)
        {
            _context = context;
            groupeRepository = new GroupeRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] GroupesFilterModel filterModel)
        {
            try
            {
                return Ok(groupeRepository.Filter(
                        filter: x => ((x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()))),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                //   var client = groupeRepository.GetById(filter: x => x.Id == id);
                var client = groupeRepository.GetGroupe(id);

                if (client == null)
                {
                    return NotFound();
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Groupe groupe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                groupeRepository.Create(groupe);

                await _context.SaveChangesAsync();

                return Ok(groupe);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Groupe groupe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clientInDB = groupeRepository.GetById(groupe.Id);

                if (clientInDB == null)
                {
                    return NotFound();
                }

                groupeRepository.Update(groupe);

                await _context.SaveChangesAsync();

                return Ok(groupe);
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

                var client = groupeRepository.GetById(id);

                if (client == null)
                {
                    return NotFound();
                }

                groupeRepository.Delete(client);

                _context.SaveChanges();

                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("CheckUniqueNom/{nom}")]
        public async Task<IActionResult> CheckUniqueNom(string nom)
        {
            try
            {
                return Ok(groupeRepository.CheckUniqueNom(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}