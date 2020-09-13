using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Parametrage;
using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametragesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IParametrageRepository parametrageRepository;

        public ParametragesController(ProjetBaseContext context)
        {
            _context = context;
            parametrageRepository = new ParametrageRepository(_context);
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Parametrages parametrageModel)
        {
            try
            {
                parametrageRepository.Create(parametrageModel);

                await _context.SaveChangesAsync();

                return Ok(parametrageModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> Get([FromRoute] int type)
        {
            try
            {
                var parametrage = parametrageRepository.GetById(x => x.Type == type);

                if (parametrage == null)
                {
                    return NotFound();
                }

                return Ok(parametrage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{type}")]
        public async Task<IActionResult> Put([FromRoute] int type, [FromBody]Parametrages parametrage)
        {
            try
            {
                var parametrageInDB = parametrageRepository.GetById(x => x.Type == type);

                if (parametrageInDB == null)
                {
                    return NotFound();
                }
                parametrageInDB.Contenu = parametrage.Contenu;
                parametrageRepository.Update(parametrageInDB);

                await _context.SaveChangesAsync();

                return Ok(parametrage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Generate/{type}")]
        public async Task<IActionResult> GenerateParameter([FromRoute] TypeNumerotation type)
        {
            try
            {
                var parametrage = parametrageRepository.GetById(x => x.Type == (int)TypeParametrage.numerotaion);

                if (parametrage == null)
                {
                    return NotFound();
                }
                List<Parametrage> contenu = JsonConvert.DeserializeObject<List<Parametrage>>(parametrage.Contenu);
                Parametrage numerotationInfos = contenu.Where(x => x.Type == (int)type).FirstOrDefault();

                var generatedParamter = await parametrageRepository.GenerateParameter(numerotationInfos);

                return Ok(new { data = generatedParamter });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Increment/{type}")]
        public async Task<IActionResult> Increment([FromRoute] TypeNumerotation type)
        {
            try
            {

                var result = await parametrageRepository.Increment(type);
                if (!result)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(true);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

    }
}