using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Labels;
using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly ILabelsRepository labelsRepository;

        public LabelsController(ProjetBaseContext context)
        {
            _context = context;
            labelsRepository = new LabelsRepository(_context);
        }

        //ajouter un nouveau label
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Labels labels)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (_context.Labels.Where(L => L.Label.ToUpper() == labels.Label.ToUpper()).Count() != 0)
                {
                    return Ok(false);
                }

                labelsRepository.Create(new Labels {
                    Label = labels.Label.ToUpper(),
                });

                await _context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        //modifier les information d'une label
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Labels label)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var labelInDB = labelsRepository.GetById(label.Id);

                if (labelInDB == null)
                {
                    return NotFound();
                }
                if(_context.Labels.Where(L => L.Label.ToUpper() == label.Label.ToUpper() && label.Id != L.Id).Count() != 0)
                {
                    return Ok(false);
                }
                labelInDB.Label = label.Label.ToUpper();

                labelsRepository.Update(labelInDB);


                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        //supprimer un label
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var produit = labelsRepository.GetById(id);

                if (produit == null)
                {
                    return NotFound();
                }

                labelsRepository.Delete(produit);

                _context.SaveChanges();

                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        //récupérer un label par son id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var label = labelsRepository.GetById(filter: x => x.Id == id, include: "");

                if (label == null)
                {
                    return NotFound();
                }

                return Ok(label);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        //récupérer la liste de produits
        [HttpPost]
        public async Task<IActionResult> Index([FromBody] FilterModel filterModel)
        {
            try
            {
                var userManager = new UserManager(_context);
                var currentUser =  userManager.GetCurrentUser();
                var resultat = labelsRepository.Filter(
                        filter: x => (x.Label.ToLower().Contains(filterModel.SearchQuery.ToLower())),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
                        );
                resultat.List = resultat.List.Distinct().ToList();
                return Ok(resultat);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}