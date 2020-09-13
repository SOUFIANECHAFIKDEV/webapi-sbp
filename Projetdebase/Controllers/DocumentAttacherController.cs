using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.DocumentAttacher;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentAttacherController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IDocumentAttacherRepository documentAttacherRepository;

        public DocumentAttacherController(ProjetBaseContext context)
        {
            _context = context;
            documentAttacherRepository = new DocumentAttacherRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] DocAttacherFilterModel filterModel)
        {
            try
            {
                var resultat = documentAttacherRepository.Filter(
                        filter: x => (
                        (x.Commentaire.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Designation.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                        && (filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true)
                        && (filterModel.Labels.Count() != 0 ? (filterModel.Labels.Any(item => x.LabelDocument.ToUpper().Contains(item.ToUpper()))) : true)
                        && (filterModel.Rubrique.HasValue ? (x.idRubrique == filterModel.Rubrique) : true)
                        ),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams
                        );
                return Ok(resultat);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }

        }

        //récupérer un document attacher par  id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var label = documentAttacherRepository.GetById(filter: x => x.Id == id);


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



        //ajouter un nouveau document attacher
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentAttacher documentAttacher)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                documentAttacherRepository.Create(documentAttacher);
                return Ok(documentAttacher);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] DocumentAttacher documentAttacher)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var documentAttacherInDB = documentAttacherRepository.GetById(documentAttacher.Id);

                if (documentAttacher == null)
                {
                    return NotFound();
                }

                
                await documentAttacherRepository.Update(documentAttacher);
                return Ok(documentAttacher);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        //supprimer  document attacher
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                DocumentAttacher documentAttacher = documentAttacherRepository.GetById(id);

                if (documentAttacher == null)
                {
                    return NotFound();
                }

                await documentAttacherRepository.Delete(documentAttacher);

                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }



    }
}
