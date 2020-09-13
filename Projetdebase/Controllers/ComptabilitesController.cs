using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Comptabilite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComptabilitesController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IComptabiliteRepository _comptabiliteRepository;
        public ComptabilitesController(ProjetBaseContext context)
        {
            _context = context;
            _comptabiliteRepository = new ComptabiliteRepository(_context);
        }


        [HttpPost]
        [Route("ExportComptabiliteComptes")]
        public async Task<IActionResult> ExportComptabiliteComptes([FromBody] ComptabiliteComptesFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.ExportComptabiliteComptes(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("ExportJournalVente")]
        public async Task<IActionResult> ExportJournalVente([FromBody] JournalVenteFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.ExportJournalVente(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("ExportJournalAchat")]
        public async Task<IActionResult> ExportJournalAchat([FromBody] JournalVenteFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.ExportJournalAchat(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("JournalVente")]
        public async Task<IActionResult> JournalVente([FromBody] JournalVenteFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.JournalVente(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }



        [HttpPost]
        [Route("JournalAchat")]
        public async Task<IActionResult> JournalAchat ([FromBody] JournalVenteFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.JournalAchat(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("ComptabiliteComptes")]
        public async Task<IActionResult> ComptabiliteComptes ([FromBody] ComptabiliteComptesFilter filterModel)
        {
            try
            {
                return Ok(await _comptabiliteRepository.ComptabiliteComptes(filterModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
            }
        }


    }

