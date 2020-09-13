using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InoAuthentification.Entities;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Categorie;
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
    public class CategorieController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly ICategorieRepository categorieRepository;

        public CategorieController(ProjetBaseContext context)
        {
            _context = context;
            categorieRepository = new CategorieRepository(_context);
        }

        // GET: api/Categorie
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] FilterModel filterModel)
        {
            try
            {
                return
                    Ok(categorieRepository.Filter(
                        filter: x => ((x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Code_comptable.ToLower().Contains(filterModel.SearchQuery.ToLower()))),
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
        // GET: api/Categorie/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategorie([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categorie = await _context.Categorie.FindAsync(id);

            if (categorie == null)
            {
                return NotFound();
            }

            return Ok(categorie);
        }


        // PUT: api/Categorie/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategorie([FromRoute] int id, [FromBody] Categorie categorie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categorie.Id)
            {
                return BadRequest();
            }

            try
            {
                categorieRepository.Update(categorie);
                await _context.SaveChangesAsync();
                return Ok(categorie);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategorieExists(id))
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
        public async Task<IActionResult> Post([FromBody] Categorie category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Categorie.Add(category);
                await _context.SaveChangesAsync();

                return Ok(category);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        // DELETE: api/Categorie/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategorie([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categorie = await _context.Categorie.FindAsync(id);
            if (categorie == null)
            {
                return NotFound();
            }

            _context.Categorie.Remove(categorie);
            await _context.SaveChangesAsync();

            return Ok(categorie);
        }

        private bool CategorieExists(int id)
        {
            return _context.Categorie.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("CheckUniqueNom/{nom}")]
        public async Task<IActionResult> CheckUniqueNom(string nom)
        {
            try
            {
                return Ok(categorieRepository.CheckUniqueNom(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


    }
}


