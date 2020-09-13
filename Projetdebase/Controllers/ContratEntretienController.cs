using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Outils.EXCEL;
using ProjetBase.Businnes.Repositories.ContratEntretien;
using ProjetBase.Businnes.Repositories.VisiteMaintenance;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProjetBase.Businnes.Models.FicheTechniqueModal;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratEntretienController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IContratEntretienRepository contratEntretienRepository;
        private readonly IVisiteMaintenanceRepository visiteMaintenanceRepository;

        public ContratEntretienController(ProjetBaseContext context)
        {
            _context = context;
            contratEntretienRepository = new ContratEntretienRepository(_context);
            visiteMaintenanceRepository = new VisiteMaintenanceRepository(_context);

        }

        // GET: api/Factures/All
        [HttpPost]
        [HttpPost]
        [Route("All")]
        public async Task<IActionResult> Get([FromBody] ContratEntretienModel filterModel)
        {
            try
            {
                return Ok(contratEntretienRepository.Filter(
                        pagingParams: filterModel.PagingParams,
                        filter: x => (
                        ((filterModel.Statut.HasValue ? (filterModel.Statut == (int)x.Statut) : true)) &&
                            (filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DateDebut.Date) &&
                            (filterModel.DateFin == null ? true : filterModel.DateFin >= x.DateFin.Date) &&
                           // (filterModel.IdClient.HasValue ? (x.IdClient == filterModel.IdClient) : true) &&

                           (x.Client.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                             && (filterModel.IdClient.HasValue ? x.IdClient == filterModel.IdClient : true)
                        ),
                        sortingParams: filterModel.SortingParams,
                        include: "Client"
                    ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        //ajouter un nouvelle contratEntretien
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContratEntretien contratEntretien)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var PieceJointes = JsonConvert.DeserializeObject<List<FTPieceJointes>>(contratEntretien.PiecesJointes);
                InovaFileManager.FileManager fileManager = new InovaFileManager.FileManager();
                //les fichiers ajoutés
                List<string> adeddFiles = new List<string>();
                
                PieceJointes.ForEach(file =>
                {
                    var result = !fileManager.Save(file.file, file.name);
                    
                    if (result)
                    {
                        fileManager.DeleteFiles(adeddFiles);
                        throw new Exception("One of uploaded files is not valid");
                    }
                    else
                    {
                        adeddFiles.Add(file.name);
                        file.file = "";
                    }
                });

                ContratEntretien contratEntretienAdd = new ContratEntretien
                {
                    IdClient = contratEntretien.IdClient,
                    Site = contratEntretien.Site,
                    DateDebut = contratEntretien.DateDebut,
                    DateFin = contratEntretien.DateFin,
                    Historique = contratEntretien.Historique,
                    Memos = contratEntretien.Memos,
                    PiecesJointes = JsonConvert.SerializeObject(PieceJointes),
                    Statut = contratEntretien.Statut,
                };
                _context.ContratEntretien.Add(contratEntretienAdd);
                var equipementEntretien = await contratEntretienRepository.AddEquipementContrat(contratEntretienAdd.Id, contratEntretien.EquipementContrat.ToList());

                _context.SaveChanges();
               if (contratEntretien.Statut !=(int) StatutContratEntretien.Brouillon)
                {
                    await visiteMaintenanceRepository.AddVisiteMaintenance(contratEntretienAdd);
                }
               

                return Ok(contratEntretienAdd);
           }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }


        // GET: api/contratEntretien/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contratEntretien = contratEntretienRepository.GetContratEntretien(id);

            if (contratEntretien == null)
            {
                return NotFound();
            }

            return Ok(contratEntretien);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ContratEntretien contratEntretien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contratEntretien.Id = id;
            if (id != contratEntretien.Id)
            {
                return BadRequest();
            }

            var contratEntretienInDb = contratEntretienRepository.GetContratEntretien(id);

            if (contratEntretienInDb == null)
            {
                return NotFound();
            }

            try
            {

                // Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(contratEntretienInDb, contratEntretien);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = contratEntretienRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(contratEntretienInDb.Historique);
                    var userManager = new UserManager(_context);
                    var currentUser = userManager.GetCurrentUser();
                    if (champsModify.Count() > 0)
                    {
                        hitoriques.Add(new HistoriqueModel()
                        {
                            date = DateTime.Now,
                            action = (int)ActionHistorique.Updated,
                            IdUser = currentUser.Id,
                            champs = champsModify
                        });
                        contratEntretien.Historique = JsonConvert.SerializeObject(hitoriques);
                    }
                 
                }
                var equipemntContratList = contratEntretien.EquipementContrat.ToList();
           
                contratEntretien.EquipementContrat = null;
                _context.Update(contratEntretien);

                await _context.SaveChangesAsync();
                var res = await contratEntretienRepository.deleteEquipementContrat(contratEntretienInDb.EquipementContrat.ToList());
                var equipementEntretien = await contratEntretienRepository.AddEquipementContrat(id, equipemntContratList);

                if (contratEntretien.Statut != (int)StatutContratEntretien.Brouillon)
                {
                    await visiteMaintenanceRepository.AddVisiteMaintenance(contratEntretien);
                }

                return Ok(contratEntretien);

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // DELETE: api/Contrat Entretien/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var contratEntretien = contratEntretienRepository.GetContratEntretien(id);


                if (contratEntretien == null)
                    return NotFound();

                //if (contratEntretien.Statut != (int)StatutFacture.Brouillon)
                //    return BadRequest();
                // var res = await contratEntretienRepository.deleteEquipementContrat(contratEntretien.EquipementContrat.ToList());
                contratEntretienRepository.Delete(contratEntretien);
                await _context.SaveChangesAsync();

                return Ok(contratEntretien);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
        [HttpPut("changeStatut")]
        public async Task<IActionResult> changeStatut(ChangeStatutBodyRequestContratEntretien body)
        {
            try
            {

                ContratEntretien contratEntretienInDb = contratEntretienRepository.GetContratEntretien(body.idContratEntretien);
                var statutInDb = contratEntretienInDb.Statut;
                contratEntretienInDb.Statut = (int)body.statutContratEntretien;
              
                contratEntretienRepository.Update(contratEntretienInDb);
           
                await _context.SaveChangesAsync();

                if (statutInDb == (int)StatutContratEntretien.Brouillon && body.statutContratEntretien == StatutContratEntretien.EnAttente)
                {
                    await visiteMaintenanceRepository.AddVisiteMaintenance(contratEntretienInDb);
                }
                return Ok(contratEntretienInDb);
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
                return Ok(await contratEntretienRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ExportGammeMaintenanceEquipement/{id}")]
        public async Task<IActionResult> ExportGammeMaintenanceEquipement([FromRoute] int id)
        {
            try
            {
                //var contrat = contratEntretienRepository.GetById(id, include: "Client");
                var contratEntretien = contratEntretienRepository.GetContratEntretien(id);
                return Ok(contratEntretienRepository.ExportGammeMaintenenceEquipement(contratEntretien));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }
    }

}
