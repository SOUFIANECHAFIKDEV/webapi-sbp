using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.Entities;
using InoAuthentification.UserManager.Models;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Client;
using Serilog;

namespace ProjetBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ProjetBaseContext _context;
        private readonly IClientRepository clientRepository;

        public ClientsController(ProjetBaseContext context)
        {
            _context = context;
            clientRepository = new ClientRepository(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] ClientsFilterModel filterModel)
        {
            try
            {
                return Ok(clientRepository.Filter(
                        filter: x => ((x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower()) || x.Codeclient.ToLower().Contains(filterModel.SearchQuery.ToLower()))) ,
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams,
                        include: ""
                        ));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var xclient = clientRepository.GetById(id);
                var client = clientRepository.GetById(filter: x => x.Id == id);

                if (client == null)
                {
                    return NotFound();
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                clientRepository.Create(client);

                await _context.SaveChangesAsync();

                //User user = new User
                //{
                    
                //}

                //UserManager userManager = new UserManager(_context);
                //UserModel createdUser;
                //createdUser = await userManager.CreateUserAsync(user, user.Password);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clientInDB = clientRepository.GetById(client.Id);

                if (clientInDB == null)
                {
                    return NotFound();
                }

                // Get differte between object send and stored object
                var champsModify = EntityExtensions.GetModification(clientInDB, client);

                // Add Historique
                if (champsModify.Count > 0)
                {
                    champsModify = clientRepository.ChangeIdToNameHistorique(champsModify);
                    var hitoriques = JsonConvert.DeserializeObject<List<HistoriqueModel>>(clientInDB.Historique);
                    var userManager = new UserManager(_context);
                    var currentUser =  userManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ActionHistorique.Updated,
                        IdUser = currentUser.Id,
                        champs = champsModify
                    });
                    client.Historique = JsonConvert.SerializeObject(hitoriques);
                }

                clientRepository.Update(client);


                await _context.SaveChangesAsync();
                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
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

                var client = clientRepository.GetById(id);

                if (client == null)
                {
                    return NotFound();
                }

                //clientRepository.Delete(client);

                //_context.SaveChanges();

                //   return Ok(true);
                var result = clientRepository.supprimerClient(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

       [HttpGet]
        [Route("CheckUniqueCodeClient/{codeclient}")]
        public async Task<IActionResult> CheckUniqueReference(string codeclient)
        {
            try
            {
                return Ok(clientRepository.CheckUniqueCodeClient(codeclient));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

       [HttpPost]
        [Route("memos/{id}")]
        public async Task<IActionResult> saveMemos([FromRoute] int id, [FromBody] string memos)
        {
            try
            {
                return Ok(await clientRepository.saveMemos(id, memos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [Route("UpdateMemos")]
        [HttpPost]
        public async Task<IActionResult> updateFicheTehcnique(EditMemosViewModel body)
        {
            try
            {
                var result = await clientRepository.UpdateMemos(body);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}