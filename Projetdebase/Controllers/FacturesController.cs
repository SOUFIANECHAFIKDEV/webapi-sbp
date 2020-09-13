using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.Attributes;
using InoAuthentification.UserManagers;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Outils.Pdf_Document;
using ProjetBase.Businnes.Outils.PDF_FactureParPeriod;
using ProjetBase.Businnes.Repositories;
using ProjetBase.Businnes.Repositories.Avoir;
using ProjetBase.Businnes.Repositories.Chantier;
using ProjetBase.Businnes.Repositories.Client;
using ProjetBase.Businnes.Repositories.Devis;
using ProjetBase.Businnes.Repositories.Facture;
using ProjetBase.Businnes.Repositories.Parametrage;




using Serilog;


namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturesController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IFactureRepository factureRepository;
        private IPdf_Document pDFFacture;
        private readonly IAvoirRepository avoirRepository;
        private IPdf_Document pDFAvoir;
        private readonly IParametrageRepository parametrageRepository;
        private readonly IDevisRepository devisRepository;
        private readonly IChantierRepository chantierRepository;


        public FacturesController(ProjetBaseContext context)
        {
            _context = context;
            factureRepository = new FactureRepository(_context);
            avoirRepository = new AvoirRepository(_context);
            parametrageRepository = new ParametrageRepository(_context);
            devisRepository = new DevisRepository(_context);
            chantierRepository = new ChantierRepository(_context);
            pDFFacture = new Pdf_Document();
            pDFAvoir = new Pdf_Document();
        }

        // GET: api/Factures/All
        [HttpPost]
        [HttpPost]
        [Route("All")]
        public async Task<IActionResult> Get([FromBody] FactureAllModel filterModel)
        {
            try
            {
                return Ok(factureRepository.Filter(
                        pagingParams: filterModel.PagingParams,
                        filter: x => (
                            (filterModel.DateDebut == null ? true : filterModel.DateDebut <= x.DateEcheance.Date) &&
                            (filterModel.DateFin == null ? true : filterModel.DateFin >= x.DateEcheance.Date) &&
                            (filterModel.IdChantier.HasValue ? (x.IdChantier == filterModel.IdChantier) : true) &&
                            (filterModel.Statut.Count() > 0 ? filterModel.Statut.Contains(x.Status) : true) &&
                           (x.Reference.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Chantier.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || (x.Object ?? "").ToLower().Contains(filterModel.SearchQuery.ToLower()))
                              // && (filterModel.IdClient.HasValue ? x.Chantier.IdClient == filterModel.IdClient : true)
                             && (filterModel.IdClient.HasValue ? x.IdClient == filterModel.IdClient : true) 
                        ),
                        sortingParams: filterModel.SortingParams,
                        include: "Chantier,FacturePaiements,Devis"
                    ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // GET: api/Factures/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var facture = await factureRepository.GetFacture(id);

                if (facture == null)
                {
                    return NotFound();
                }

                return Ok(facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        // POST: api/Factures/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FacturePostModel facturePostModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                factureRepository.Create(facturePostModel.Facture);
                if (facturePostModel.Facture.IdDevis != null && facturePostModel.Facture.Status != (int)StatutFacture.Brouillon)
                {
                    var devis = devisRepository.GetDevis((int)facturePostModel.Facture.IdDevis);
                    devis.Status = StatutDevis.Acceptee;
                   devis.Chantier.Statut = StatutChantier.Accepte;
                    _context.Devis.Update(devis);
                }

                await _context.SaveChangesAsync();

                if (facturePostModel.Facture.IdChantier != null && facturePostModel.Facture.IdDevis == null && facturePostModel.Facture.Status != (int)StatutFacture.Brouillon)
                {
                    var chantier = _context.Chantier.SingleOrDefault(x=> x.Id == facturePostModel.Facture.IdChantier);
                    chantier.Statut = chantier.Statut == StatutChantier.Termine ? StatutChantier.Accepte : chantier.Statut;
                    _context.Update(chantier);
                    await _context.SaveChangesAsync();
                }

              
                if (facturePostModel.FicheInterventionIds.Count() > 0)
                {
                    factureRepository.TransfertFichesInterventionToFacture(facturePostModel.Facture.Id, facturePostModel.FicheInterventionIds);
                }
              
                return Ok(facturePostModel.Facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // PUT: api/Factures/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Facture facture)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                facture.Id = id;
                var factureInDB = factureRepository.GetById(facture.Id);
                if(facture.IdClient == null)
                {
                    facture.IdClient = factureInDB.IdClient;
                }
               
                var champsModify = EntityExtensions.GetModification(factureInDB, facture);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = factureRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(factureInDB.Historique);
                    var userManager = new UserManager(_context);
                    var ccurrentUser = userManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = ccurrentUser.Id,
                        champs = champsModify
                    });
                    facture.Historique = JsonConvert.SerializeObject(hitoriques);
                }

                if (facture.IdDevis != null && facture.Status != (int)StatutFacture.Brouillon)
                {
                   facture.Devis.Status = StatutDevis.Acceptee;
                    facture.Chantier.Statut = StatutChantier.Accepte;

                }
                if (facture.IdChantier != null && facture.IdDevis == null && facture.Status != (int)StatutFacture.Brouillon)
                {
                   facture.Chantier.Statut = facture.Chantier.Statut == StatutChantier.Termine ? StatutChantier.Accepte : facture.Chantier.Statut;

                }

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

        // DELETE: api/Factures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var facture = factureRepository.GetById(id);
                

                if (facture == null)
                    return NotFound();

                if (facture.Status != (int)StatutFacture.Brouillon)
                    return BadRequest();

                factureRepository.Delete(facture);
                await _context.SaveChangesAsync();

                return Ok(facture);
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
                return Ok(await factureRepository.SaveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        // Annuler facutre
        [HttpPost]
        [Route("AnnulerFacture/{id}")]
        public async Task<IActionResult> AnnulerFacture([FromRoute]int id, [FromBody] Avoir avoir)
        {
            try
            {
                var facture = await factureRepository.GetFacture(id);

                if (facture == null)
                {
                    return NotFound();
                }

                var resAvoir = await factureRepository.AnnulerFacture(facture, avoir);
                await _context.SaveChangesAsync();

                return Ok(avoir);
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
                return Ok(factureRepository.CheckUniqueReference(reference));
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
                    iTextSharp.text.Document doc = pDFFacture.CreateDocument();
                    pDFFacture.GeneratePDFWriter(doc, output);
                    pDFFacture.OpenDocument(doc);
                    var facture = await factureRepository.GetFacture(id);
                    var param = parametrageRepository.GetParametrageDocument();
                    pDFFacture.Header(doc, facture, param);
                    //pDFFacture.HeaderAdresee(doc, facture);
                    pDFFacture.BodyArticles(doc, facture);
                    pDFFacture.calculTva(doc, facture);

                    pDFFacture.PiedPage(doc, facture, param);
                    pDFFacture.conditionREG(doc, facture, param);
                    pDFFacture.CloseDocument(doc);
                    return Ok(output.ToArray());
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        private IPDFFactureParPeriod pDFFacturePatPeriode;

        private async Task<List<Chantier>> selectChantierTOExportDocs(int idChantiers)
        {
            List<Chantier> chantiers = new List<Chantier>();
            IChantierRepository chantiertRepository = new ChantierRepository(_context);
            if (idChantiers != -1)
            {
                var chantier = await chantiertRepository.GetChantier(idChantiers);
                chantiers.Add(chantier);

            }
            else
            {
                chantiers = await chantiertRepository.GetAllChantier();
            }
            return chantiers;
        }


        private async Task<List<Client>> selectClientTOExportDocs(int idClient)
        {
            List<Client> clients = new List<Client>();
            IClientRepository clientRepository = new ClientRepository(_context);
            if (idClient != -1)
            {
                clients.Add(clientRepository.GetById((int)idClient));
            }
            else
            {
                clients = await clientRepository.GetAllClient();
            }
            return clients;
        }

        private async Task<List<byte[]>> GeneratePDFsFactures(List<Facture> ListFacturesClient)
        {

            List<byte[]> bytes = new List<byte[]>();

            var param = parametrageRepository.GetParametrageDocument();

            iTextSharp.text.Document doc = pDFFacture.CreateDocument();
            MemoryStream output = new MemoryStream();
            pDFFacture.GeneratePDFWriter(doc, output);
            using (output)
            {
                foreach (var f in ListFacturesClient)
                {
                    var facture = await factureRepository.GetFacture(f.Id);

                    pDFFacture.OpenDocument(doc);
                    pDFFacture.Header(doc, facture, param);
                  //  pDFFacture.HeaderAdresee(doc, facture);
                    pDFFacture.BodyArticles(doc, facture);
                    pDFFacture.calculTva(doc, facture);
                    pDFFacture.conditionREG(doc, facture, param);
                    pDFFacture.PiedPage(doc, facture, param);
                    doc.NewPage();

                }
                pDFFacture.CloseDocument(doc);
            }

            bytes.Add(output.ToArray());
            return bytes;
        }

        private async Task<List<byte[]>> GeneratePDFsAvoirs(List<Avoir> ListAvoirsClient)
        {

            List<byte[]> bytes = new List<byte[]>();
            var param = parametrageRepository.GetParametrageDocument();
            Document doc = pDFAvoir.CreateDocument();
            MemoryStream output = new MemoryStream();
            pDFAvoir.GeneratePDFWriter(doc, output);
            using (output)
            {
                foreach (var a in ListAvoirsClient)
                {
                    var avoir =  avoirRepository.GetAvoir(a.Id);
                    pDFAvoir.OpenDocument(doc);
                    pDFAvoir.Header(doc, avoir, param);
                   // pDFAvoir.HeaderAdresee(doc, avoir);
                    pDFAvoir.BodyArticles(doc, avoir);
                    pDFAvoir.calculTva(doc, avoir);
                    pDFAvoir.conditionREG(doc, avoir, param);
                    pDFAvoir.PiedPage(doc, avoir, param);
                    doc.NewPage();

                }
                pDFAvoir.CloseDocument(doc);
            }
            bytes.Add(output.ToArray());
            return bytes;
        }


        [HttpPost]
        [Route("exportFactureParPeriod")]
        public async Task<IActionResult> exportFactureParPeriod([FromBody] ExportFactureEtAvoirParPeriodModel exportFactureParPeriod)
        {
            try
            {
                #region sélectionner les clients qui nous voulons exporter leur documents
               // List<Chantier> chantiers = await selectChantierTOExportDocs((int)exportFactureParPeriod.IdChantier);
                List<Client> clients = await selectClientTOExportDocs((int)exportFactureParPeriod.IdClient);


                #endregion

                #region initialisation des variables
                List<byte[]> bytes = new List<byte[]>();
                List<byte[]> filesDetail = new List<byte[]>();
                //...
                pDFFacturePatPeriode = new PDFFactureParPeriod();
                iTextSharp.text.Document doc = pDFFacturePatPeriode.CreateDocument();
                MemoryStream output = new MemoryStream();
                pDFFacturePatPeriode.GeneratePDFWriter(doc, output);
                #endregion

                using (output)
                {
                    foreach (var client in clients)
                    {
                        #region initialiser les statuts des factures recuperé
                        List<int> stauts = new List<int>();
                        stauts.Add((int)StatutFacture.Enretard);
                        stauts.Add((int)StatutFacture.Encours);
                        stauts.Add((int)StatutFacture.Cloture);
                        #endregion

                        #region récuperer les factures entre deux dates pour tout les chantier ou pour un chantier spésifique
                        var ListFacturesClient = factureRepository.GetFacturesClient(client.Id, stauts, exportFactureParPeriod.DateDebut, exportFactureParPeriod.DateFin);
                        #endregion

                        #region récuperer les avoirs entre deux dates pour tout les chantier ou pour un chantier spésifique
                        IAvoirRepository avoirRepository = new AvoirRepository(_context);
                        List<Avoir> listAvoirsClient = await avoirRepository.GetAvoirsClient(client.Id, stauts, exportFactureParPeriod.DateDebut, exportFactureParPeriod.DateFin);
                        #endregion

                        #region récuperer le paramètres du document du client
                        var param = parametrageRepository.GetParametrageDocument();

                        #endregion

                        #region ouvrir le document
                        doc.SetMargins(50f, 50f, 40f, 75f);
                        pDFFacturePatPeriode.OpenDocument(doc);
                        #endregion

                        #region ajouter l'entête du document
                        pDFFacturePatPeriode.Header(doc, client, exportFactureParPeriod, param, 1);
                        #endregion

                        #region créer un nouveau document
                        pDFFacturePatPeriode.BodyArticles(doc, ListFacturesClient, listAvoirsClient);
                        #endregion

                        #region créer un nouveau docuemnt
                        doc.NewPage();
                        #endregion

                        #region  vérifier si la possibilité d'exporter le details des facture est positive
                        if (exportFactureParPeriod.ExporterFacturesChantier)
                        {


                            #region création les document des factures
                            if (ListFacturesClient.Count() > 0)
                            {
                                filesDetail.AddRange(await GeneratePDFsFactures(ListFacturesClient));
                            }
                            #endregion

                            #region création des document des avoirs
                            if (listAvoirsClient.Count() > 0)
                            {
                                filesDetail.AddRange(await GeneratePDFsAvoirs(listAvoirsClient));
                            }
                           

                            #endregion
                        }
                        #endregion
                    }

                    #region fermer le document de la list des factures et des avoirs
                    pDFFacturePatPeriode.CloseDocument(doc);
                    bytes.Add(output.ToArray());
                    #endregion
                }


                return Ok(new exportFactureParPeriodModel
                {
                    FacturesParPeriod = bytes,
                    FacturesAndAvoirsDetail = filesDetail
                });

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("exportreleveRelanceFacture")]
        public async Task<IActionResult> exportreleveRelanceFacture([FromBody] exportReleveRelanceFactureModel exportFactureParPeriod)
        {
            try
            {
                #region sélectionner les clients qui nous voulons exporter leur documents
                //List<Chantier> chantiers = await selectChantierTOExportDocs((int)exportFactureParPeriod.IdChantier);
                List<Client> clients = await selectClientTOExportDocs((int)exportFactureParPeriod.IdClient);

                #endregion

                #region initialisation des variables
                List<byte[]> bytes = new List<byte[]>();
                List<byte[]> filesDetail = new List<byte[]>();
                //...
                pDFFacturePatPeriode = new PDFFactureParPeriod();
                iTextSharp.text.Document doc = pDFFacturePatPeriode.CreateDocument();
                MemoryStream output = new MemoryStream();
                pDFFacturePatPeriode.GeneratePDFWriter(doc, output);
                #endregion

                using (output)
                {
                    foreach (var client in clients)
                    {
                        #region initialiser les statuts des factures recuperé
                        List<int> stauts = new List<int>();
                        stauts.Add((int)StatutFacture.Enretard);
                        #endregion

                        #region récuperer les factures entre deux dates pour tout les client ou pour un client spésifique
                        var ListFacturesChantier = factureRepository.GetFacturesClient(client.Id, stauts, null, null);
                        #endregion
                        var param = parametrageRepository.GetParametrageDocument();
                        pDFFacturePatPeriode.OpenDocument(doc);
                        pDFFacturePatPeriode.HeaderFactureRelance(doc, client, exportFactureParPeriod, param, 1);
                        pDFFacturePatPeriode.BodyArticlesRelence(doc, ListFacturesChantier, new List<Avoir>());
                        doc.NewPage();
                        if (exportFactureParPeriod.ExporterFacturesChantier)
                        {
                            if (ListFacturesChantier.Count != 0)
                            {
                                filesDetail.AddRange(await GeneratePDFsFactures(ListFacturesChantier));
                            }
                        }
                    }
                    pDFFacturePatPeriode.CloseDocument(doc);
                    bytes.Add(output.ToArray());
                }

                return Ok(new exportFactureParPeriodModel
                {
                    FacturesParPeriod = bytes,
                    FacturesAndAvoirsDetail = filesDetail
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("sendEmail/{idFacture}")]
        public async Task<IActionResult> sendEmail(int idFacture, [FromBody] InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {
                return Ok(await factureRepository.SendEmail(idFacture, MailParams));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateFactureSituation")]
        public async Task<IActionResult> factureSituation([FromBody] FactureSituationModal factureSituationModal)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Facture facture = await factureRepository.CreateFactureSituation(factureSituationModal);

                return Ok(facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateAcompteSituation")]
        public async Task<IActionResult> acompteSituation([FromBody] FactureAcomptesModal factureAcompteModal)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Facture facture = await factureRepository.CreateFactureAcompte(factureAcompteModal); 

                return Ok(facture);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        
    }
}