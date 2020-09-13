using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InoAuthentification.Attributes;
using InoAuthentification.Entities;
using InoAuthentification.JwtManagers.Models;
using InoAuthentification.UserManager.Models;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Account;
using System.Net.Mail;
using Serilog;
using System.Net;
using Microsoft.Extensions.Configuration;
using ProjetBase.Businnes.Entities;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Enum;

namespace Projet__de_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly ProjetBaseContext _context;
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration _configuration;

        public AccountController(ProjetBaseContext context, IConfiguration iConfig)
        {
            _context = context;
            accountRepository = new BonCommandeFournisseurtRepository(_context);
            _configuration = iConfig;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                //var user = accountRepository.GetById(filter: x => x.Id == id && !x.IsDeleted);

                //if (user == null)
                //{
                //    return NotFound();
                //}

                //return Ok(user);
                var user = accountRepository.GetById(filter: x => x.Id == id );
               // var profile = _context.UserProfile.Where(u => u.Iduser == user.Id).ToList();
                //user.UserProfile = profile;
                //foreach (var UserProfil in user.UserProfile)
                //{
                //    var Profil = await _context.Profile.Where(x => x.Id == UserProfil.Idprofile).FirstOrDefaultAsync();
                //    UserProfil.IdprofileNavigation = Profil;
                //}
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        public IActionResult Index([FromBody] AccountFilterModel filterModel)
        {
            try
            {
                var result = accountRepository.Filter(
                        filter: x => ((
                        x.Nom.ToLower().Contains(filterModel.SearchQuery.ToLower())
                        || x.Prenom.ToLower().Contains(filterModel.SearchQuery.ToLower())
                       || x.Username.ToLower().Contains(filterModel.SearchQuery.ToLower()))
                        &&
                           // (filterModel.ProfileType != 0 ? x.IdProfile == filterModel.ProfileType : true)
                        (filterModel.ProfileType.Count() > 0 ? filterModel.ProfileType.Contains(x.IdProfile) : true) 
                             && !x.IsDeleted),
                          
                        pagingParams: filterModel.PagingParams,
                        sortingParams: filterModel.SortingParams
                        );

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatUser([FromBody] User user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserManager userManager = new UserManager(_context);

            UserModel createdUser;
            try
            {
                createdUser = await userManager.CreateUserAsync(user, user.Password);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
            return CreatedAtAction("CreatUser", new { createdUser.Id }, createdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = accountRepository.GetById(x => x.Id == id && !x.IsDeleted);
                if (user == null)
                {
                    return NotFound();
                }

                user.IsDeleted = true;
                accountRepository.Update(user);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserManager userManager = new UserManager(_context);
            InoAuthentification.UserManager.Models.TokenModel tokenModel;

            try
            {
                tokenModel = userManager.Login(loginModel);


                /* Update la derniere connection */
                if (tokenModel != null)
                {
                    var user = _context.User.Where(x => x.Id == tokenModel.User.Id).FirstOrDefault();
                    user.Dernierecon = DateTime.Now;
                    accountRepository.Update(user);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction("Login", new { tokenModel.User.Id }, tokenModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpPost]
        [Route("recoverAccount")]
        public async Task<IActionResult> RecoverAccount([FromBody] RecoverPasswordModel recoverPasswordModel)
        {
            try
            {
                var config = _configuration.GetSection("ConfigMessagerie");
                var configMessagrie = new ConfigMessagerie()
                {
                    Serveur = config.GetSection("Serveur").Value,
                    Port = Convert.ToInt32(config.GetSection("port").Value),
                    Ssl = Convert.ToInt32(config.GetSection("ssl").Value),
                    Username = config.GetSection("username").Value,
                    Password = config.GetSection("password").Value
                };
                var user = accountRepository.CheckExistsEmail(recoverPasswordModel.email);
                if (user != null)
                {
                    var newPassword = Guid.NewGuid().ToString();
                    UserManager userManager = new UserManager(_context);
                    await userManager.changePassword(user.Id, newPassword);
                    recoverPasswordModel.body = string.Format(recoverPasswordModel.body, newPassword);
                    return Ok(await accountRepository.SendRecoverEmail(recoverPasswordModel, configMessagrie));
                }
                else
                {
                    NotFound();
                }
                return new BadRequestResult();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return new BadRequestResult();
            }
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> changePassword(ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserManager userManager = new UserManager(_context);
            var result = false;
            try
            {
                result = await userManager.changePassword(changePasswordModel.idUser, changePasswordModel.password);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
            return Ok(result);
        }

        [HttpPut]
        [Route("updateUserInfos")]
        public async Task<IActionResult> updateUserInfos([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userInDB = _context.User.Where(w => w.Id == user.Id).FirstOrDefault();

            if (userInDB == null)
            {
                return NotFound();
            }

            var champsModify = accountRepository.GetModification(userInDB, user);

            // Add Historique
            if (champsModify.Count > 0)
            {
                try
                {
                    champsModify = accountRepository.ChangeHistoriqueAttributsNames(champsModify);
                    var hitoriques = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoriqueModel>>(userInDB.Historique);
                    var UserManager = new UserManager(_context);
                    var currentUser = UserManager.GetCurrentUser();
                    hitoriques.Add(new HistoriqueModel()
                    {
                        date = DateTime.Now,
                        action = (int)ProjetBase.Businnes.Enum.ActionHistorique.Updated,
                        IdUser = currentUser.Id,
                        champs = champsModify
                    });
                    user.Historique = Newtonsoft.Json.JsonConvert.SerializeObject(hitoriques);
                }
                catch (Exception)
                {

                    return new BadRequestResult();
                }
            }
            else
            {
                user.Historique = userInDB.Historique;
            }

            UserManager userManager = new UserManager(_context);

            UserModel createdUser;
            try
            {
                createdUser = await userManager.updateUserInfos(user);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
            return Ok(createdUser);
        }

        [HttpGet]
        [Route("CheckUniqueUserName/{userName}")]
        public IActionResult CheckUniqueUserName(string userName)
        {
            try
            {
                var currentUser = EntityExtensions.GetCurrentUser(_context);
                return Ok(accountRepository.CheckUniqueUserName(userName));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getUserClientInfos/{id}")]
        public IActionResult GetUserClientInfos([FromRoute] int id)
        {
            try
            {
                return Ok(accountRepository.getUserClientInfos(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}