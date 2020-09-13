using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Chantier;
using ProjetBase.Businnes.Repositories.Facture;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChantierController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IChantierRepository chantierRepository;
        private readonly IFactureRepository factureRepository;

        public ChantierController(ProjetBaseContext context)
        {
            _context = context;
            chantierRepository = new ChantierRepository(_context);
            factureRepository = new FactureRepository(_context);

        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] ChantierFilterModel filterModel)
        {
            try
            {
                return
                    Ok(chantierRepository.Filter(
                        filter: x => (
                        x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower())
                        && (filterModel.statut != null ? x.Statut == filterModel.statut : true)
                         && (filterModel.ClientId.HasValue ? x.IdClient == filterModel.ClientId : true)
                        ),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: "Client"
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChantier([FromRoute] int id)
        {
            try

            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var chantier = chantierRepository.getChantier(id);

                if (chantier == null)
                {
                    return NotFound();
                }

                return Ok(chantier);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChantier([FromRoute] int id, [FromBody] Chantier chantier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chantier.Id)
            {
                return BadRequest();
            }

            if (chantierRepository.GetById(chantier.Id) == null)
            {
                return NotFound();
            }

            try
            {
                chantierRepository.Update(chantier);
                await _context.SaveChangesAsync();
                return Ok(chantier);

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Chantier chantier)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                chantier.TauxAvancement = 0;
                _context.Chantier.Add(chantier);
                await _context.SaveChangesAsync();

                return Ok(chantier);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChantier([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chantier = await _context.Chantier.FindAsync(id);
            if (chantier == null)
            {
                return NotFound();
            }

            var result = chantierRepository.supprimerChantier(id);
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("CheckUniqueName/{nom}")]
        public async Task<IActionResult> CheckUniqueName(string nom)
        {
            try
            {
                return Ok(chantierRepository.CheckUniqueName(nom));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("documentation/{id}")]
        public async Task<IActionResult> UpdateDocumentation([FromRoute] int id, [FromBody] Documentation documentation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Chantier chantier = chantierRepository.GetById(id);

            if (chantier == null)
            {
                return NotFound();
            }

            try
            {
                var docs = JsonConvert.DeserializeObject<List<ChantierDocs>>(documentation.docs);
                if (docs.Where(d => d.type == (int)FixedDocsType.OrdreService).Count() != 0)
                {
                    chantier.Statut = StatutChantier.Accepte;
                }
                chantier.Documentation = documentation.docs;
                chantierRepository.Update(chantier);
                await _context.SaveChangesAsync();
                return Ok(chantier);

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("changeStatut")]
        public async Task<IActionResult> changeStatut(ChangeStatutBodyRequest body)
        {
            try
            {
                var result = chantierRepository.changeStatut(body);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("nbDocuments/{idChantier}")]
        public async Task<IActionResult> nbDocuments([FromRoute] int idChantier)
        {
            try
            {
                return Ok(chantierRepository.nbDocuments(idChantier));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetRecapitulatifFinancier/{idChantier}")]
        public async Task<IActionResult> GetRecapitulatifFinancier([FromRoute] int idChantier)
        {
            try
            {
                return Ok(chantierRepository.GetRecapitulatifFinancier(idChantier));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("GetRetenueGarantie/{idChantier}")]
        public async Task<IActionResult> GetRetenueGarantie([FromRoute] int idChantier)
        {
            try
            {
                return Ok(chantierRepository.GetRetenueGarantie(idChantier));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpPut("changementTauxAvencement")]
        public async Task<IActionResult> changementTauxAvencement(changementTauxAvencement body)
        {
            try
            {
                var chantierInDb = chantierRepository.getChantier(body.idChantier);
                chantierInDb.TauxAvancement = body.tauxAvencement;
                chantierRepository.Update(chantierInDb);
                await _context.SaveChangesAsync();
                return Ok(chantierInDb);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPut("changeStatutRetenueGarantie")]
        public async Task<IActionResult> changeStatutRetenueGarantie(ChangeStatusRetenueGarantieResponse body)
        {
            try
            {
                var facture = factureRepository.GetById(body.idFacture);
                facture.StatusGarantie = body.StatusRetenueGarantie;
                factureRepository.Update(facture);
                await _context.SaveChangesAsync();
                return Ok(facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
