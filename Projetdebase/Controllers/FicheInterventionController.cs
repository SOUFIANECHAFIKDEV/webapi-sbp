using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Outils.Pdf_intervention;
using ProjetBase.Businnes.Repositories.Account;
using ProjetBase.Businnes.Repositories.FicheIntervention;
using ProjetBase.Businnes.Repositories.Parametrage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace ProjetBase.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FicheInterventionController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IFicheInterventionRepository ficheInterventionRepository;
        private readonly IParametrageRepository parametrageRepository;
        private IPdfIntervention PdfIntervention;
        private readonly IAccountRepository accountRepository;

        public FicheInterventionController(ProjetBaseContext context)
        {
            _context = context;
            ficheInterventionRepository = new FicheInterventionRepository(_context);
            parametrageRepository = new ParametrageRepository(_context);
            accountRepository = new BonCommandeFournisseurtRepository(_context);


        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] FicheInterventionFilterModel filterModel)
        {
            try
            {
                return Ok(ficheInterventionRepository.Filter(

                  filter: x => (
                  (x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                   && ((filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true))
                  && ((filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Status) : true))
                  && ((filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DateFin.Date))
                  && ((filterModel.DateFin == null ? true : filterModel.DateFin >= x.DateFin.Date))
                  && ((!filterModel.IdClient.HasValue) ? true : x.Chantier.IdClient == filterModel.IdClient)
                 && ((!filterModel.idTechnicien.HasValue) ? true : x.InterventionTechnicien.Where(T => T.IdTechnicien == filterModel.idTechnicien).Count() != 0)
                  ),
                     pagingParams: filterModel.PagingParams,
                   sortingParams: filterModel.SortingParams
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var ficheIntervention = ficheInterventionRepository.GetFicheIntervention(id);

                if (ficheIntervention == null)
                {
                    return NotFound();
                }

                return Ok(ficheIntervention);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FicheIntervention ficheIntervention)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ficheIntervention.DateCreation = DateTime.Now;
                //if(ficheIntervention.Status != StatutFicheIntervention.Brouillon)
                //{
                //    ficheIntervention.idAgendaGoogle = ficheInterventionRepository.AddEventToGoogleAgenda(ficheIntervention);

                //}
                ficheInterventionRepository.Create(ficheIntervention);
                await _context.SaveChangesAsync();
                
                return Ok(ficheIntervention);
            }
            catch (Exception ex)
            {

                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFicheIntervention([FromRoute] int id, [FromBody] FicheIntervention ficheIntervention)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ficheIntervention.Id)
            {
                return BadRequest();
            }

            var ficheInterventionInDB = ficheInterventionRepository.GetFicheIntervention(ficheIntervention.Id);

            if (ficheInterventionInDB == null)
            {
                return NotFound();
            }

            try
            {

                // Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(ficheInterventionInDB, ficheIntervention);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = ficheInterventionRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(ficheInterventionInDB.Historique);
                    var userManager = new UserManager(_context);
                    var currentUser = userManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = currentUser.Id,
                        champs = champsModify
                    });
                    ficheIntervention.Historique = JsonConvert.SerializeObject(hitoriques);
                }

                ficheIntervention.Id = id;

                ficheInterventionRepository.Update(ficheIntervention);

                List<InterventionTechnicien> interventionTechnicien = _context.InterventionTechnicien.Where(I => I.IdFicheIntervention == ficheIntervention.Id).ToList();

                interventionTechnicien.ForEach(IT =>
                {
                    var result = ficheIntervention.InterventionTechnicien.Where(x => x.IdTechnicien == IT.IdTechnicien).SingleOrDefault();
                    if (result == null)
                    {
                        _context.InterventionTechnicien.Remove(IT);
                    }
                });

                await _context.SaveChangesAsync();
                return Ok(ficheIntervention);

            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
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

                var ficheIntervention = ficheInterventionRepository.GetById(id);

                if (ficheIntervention == null)
                {
                    return NotFound();
                }

                //suprrimer l evenement de la  fiche intervention dans l agenda  google
                ficheInterventionRepository.DeleteEventToGoogleAgenda(ficheIntervention);


                ficheInterventionRepository.Delete(ficheIntervention);

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
        [Route("CheckUniqueReference/{reference}")]
        public async Task<IActionResult> CheckUniqueReference(string reference)
        {
            try
            {
                return Ok(ficheInterventionRepository.CheckUniqueReference(reference));
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

                var ficheIntervention = ficheInterventionRepository.GetFicheIntervention(id);

                var param = parametrageRepository.GetParametrageDocument();
                PdfIntervention = new PdfIntervention(ficheIntervention, param);
                return Ok(PdfIntervention.GenerateDocument());

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("FicheInterventionChantier/{id}")]
        public async Task<IActionResult> FicheInterventionClient(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ListFicheIntervention = ficheInterventionRepository.GetFicheInterventionsChantier(id);
            return Ok(ListFicheIntervention);

        }

        [HttpPost]
        [Route("sendEmail/{idFicheIntervention}")]
        public async Task<IActionResult> sendEmail(int idFicheIntervention, [FromBody] InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {

                return Ok(await ficheInterventionRepository.SendEmail(idFicheIntervention, MailParams));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("changeStatut")]
        public async Task<IActionResult> changeStatut(ChangeStatutBodyRequestFicheIntervention body)
        {
            try
            {
                FicheIntervention ficheInterventionInDb = ficheInterventionRepository.GetFicheIntervention(body.idficheIntervention);
                ficheInterventionInDb.Status = body.statutficheIntervention;
                ficheInterventionRepository.Update(ficheInterventionInDb);
                return Ok(await _context.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> saveMemos([FromRoute] int id, [FromBody] string memos)
        {
            try
            {
                return Ok(await ficheInterventionRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
