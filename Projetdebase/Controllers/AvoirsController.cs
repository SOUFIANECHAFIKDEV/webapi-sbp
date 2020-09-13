using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.Attributes;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Outils.Pdf_Document;
using ProjetBase.Businnes.Repositories;
using ProjetBase.Businnes.Repositories.Avoir;
using ProjetBase.Businnes.Repositories.Parametrage;

using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class AvoirsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IAvoirRepository avoirRepository;
        private IPdf_Document pDFAvoir;
        private readonly IParametrageRepository parametrageRepository;
        

        public AvoirsController(ProjetBaseContext context)
        {
            _context = context;
            avoirRepository = new AvoirRepository(_context);
            parametrageRepository = new ParametrageRepository(_context);
            pDFAvoir = new Pdf_Document();

        }

        // GET: api/Avoirs/All
        [HttpPost]
        [Route("All")]
        public IActionResult Get([FromBody] AvoirAllModel filterModel)
        {
            try
            {
                return Ok(avoirRepository.Filter(
                        filter: x => (
                            (filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DateEcheance.Date) &&
                            (filterModel.DateFin == null ? true : filterModel.DateFin >= x.DateEcheance.Date) &&
                            (filterModel.IdChantier.HasValue ? x.IdChantier == filterModel.IdChantier : true) &&
                              (filterModel.IdClient.HasValue ? x.IdClient == filterModel.IdClient : true) &&
                            (x.Reference.Contains(filterModel.SearchQuery.ToLower()) || x.Chantier.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower())  || (x.Object ?? "").ToLower().Contains(filterModel.SearchQuery.ToLower())) &&
                            (filterModel.Statut.Count() > 0 ? filterModel.Statut.Contains(x.Status) : true) 
                            && (filterModel.MaxTotal.HasValue ? (x.Total * -1) <= filterModel.MaxTotal : true) 
                     
                        ),
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: "Chantier"
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        // GET: api/Avoirs/5
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var avoir = avoirRepository.GetAvoir( id);

                if (avoir == null)
                {
                    return NotFound();
                }

                return Ok(avoir);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // POST: api/Avoirs/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Avoir avoir)
        {
            try
            {
                 avoirRepository.Create(avoir);

                await _context.SaveChangesAsync();

                return Ok(avoir);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // PUT: api/Avoirs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Avoir avoir)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                avoir.Id = id;
                var avoirInDB = avoirRepository.GetById(avoir.Id);
                if (avoir.IdClient == null)
                {
                    avoir.IdClient = avoirInDB.IdClient;
                }
                var champsModify = EntityExtensions.GetModification(avoirInDB, avoir);
                
                if (champsModify.Count() > 0)
                {
                    champsModify = avoirRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(avoirInDB.Historique);
                    var userManager = new UserManager(_context);
                    var currentUser = EntityExtensions.GetCurrentUser(_context);
             
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = currentUser.Id,
                        champs = champsModify
                    });
                    avoir.Historique = JsonConvert.SerializeObject(hitoriques);
                }
                

                avoirRepository.Update(avoir);
                await _context.SaveChangesAsync();
                return Ok(avoir);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // DELETE: api/Avoirs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var avoir = avoirRepository.GetById(id);
                
                if (avoir == null)
                    return NotFound();

                if (avoir.Status != (int)StatutAvoir.Brouillon)
                    return BadRequest();

                avoirRepository.Delete(avoir);
                await _context.SaveChangesAsync();

                return Ok(avoir);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // Post : les notes interne
        [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> SaveMemos([FromRoute] int id, [FromBody] string memos)
        {
            try
            {
                return Ok(await avoirRepository.SaveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GeneratePDF/{id}")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    iTextSharp.text.Document doc = pDFAvoir.CreateDocument();
                    pDFAvoir.GeneratePDFWriter(doc, output);
                    pDFAvoir.OpenDocument(doc);
                    var avoir =  avoirRepository.GetAvoir(id);
                    var param = parametrageRepository.GetParametrageDocument();
                    pDFAvoir.Header(doc, avoir, param);
                   // pDFAvoir.HeaderAdresee(doc, avoir);
                    pDFAvoir.BodyArticles(doc, avoir);
                    pDFAvoir.calculTva(doc, avoir);
                    pDFAvoir.PiedPage(doc, avoir, param);
                    pDFAvoir.conditionREG(doc, avoir, param);
                    pDFAvoir.CloseDocument(doc);
                    return Ok(output.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        //Generate reference
        [HttpPost]
        [Route("GenerateReference")]
        public IActionResult CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(avoirRepository.CheckUniqueReference(reference));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        public List<Avoir> GetAvoirsChantier(List<int> status, int IdChantier, DateTime? DateDebut, DateTime? DateFin)
        {
            var result = avoirRepository.Filter(
                     filter: x => (
                         (DateDebut == null ? true : DateDebut <= x.DateEcheance.Date) &&
                         (DateFin == null ? true : DateFin >= x.DateEcheance.Date) &&
                         (x.IdChantier == IdChantier) &&
                         (status.Where(s => s == x.Status).Count() == 1) 
                     ),
                     pagingParams: null,
                     sortingParams: null,
                     include: "Chantier"
                     );
            return result.List;
        }


    }
}