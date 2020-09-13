using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InoAuthentification.Entities;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.ModeReglement;
using Serilog;
using ProjetBase.Businnes.Entities;
using InoAuthentification.Attributes;
using InoAuthentification.JwtManagers.Models;
using InoAuthentification.UserManager.Models;
using InoAuthentification.UserManagers;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Account;
using System.Net.Mail;
using System.Net;
using ProjetBase.Businnes.Contexts;


namespace Projetdebase.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
   
    public class ModelRegelementController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IModeReglementRepository modelRepository;

        public ModelRegelementController(ProjetBaseContext context)
        {
            _context = context;
            modelRepository = new ModeReglementRepository(_context);
        }

        // GET: api/ModeReglement
        [HttpPost]
        public IActionResult Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(modelRepository.Filter(
                        filter: x => x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) && !x.IsDeleted,
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

        // GET: api/Modereglement/5
        [HttpGet("{id}")]
        public IActionResult GetModereglement([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var moderegleemnt = modelRepository.GetById(x=> x.Id == id && !x.IsDeleted); 

            if (moderegleemnt == null)
            {
                return NotFound();
            }

            return Ok(moderegleemnt);
        }


        // PUT: api/ModeReglement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutmodeReglement([FromRoute] int id, [FromBody] ModeReglement modeReglement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modeReglement.Id)
            {
                return BadRequest();
            }

            try
            {
                modelRepository.Update(modeReglement);
                await _context.SaveChangesAsync();
                return Ok(modeReglement);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModeReglementExists(id))
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
        public async Task<IActionResult> Post([FromBody] ModeReglement ModeReglemen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.ModeReglement.Add(ModeReglemen);
                await _context.SaveChangesAsync();

                return Ok(ModeReglemen);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        // DELETE: api/ModeReglemen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModeReglemen([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ModeReglemen = await _context.ModeReglement.FindAsync(id);
            if (ModeReglemen == null)
            {
                return NotFound();
            }

            ModeReglemen.IsDeleted = true;

            modelRepository.Update(ModeReglemen);
            await _context.SaveChangesAsync();

            return Ok(ModeReglemen);
        }

        private bool ModeReglementExists(int id)
        {
            return _context.ModeReglement.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}


