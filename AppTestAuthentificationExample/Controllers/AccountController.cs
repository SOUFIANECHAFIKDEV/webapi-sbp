using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppTestAuthentificationExample.Contexts;
using InoAuthentification.Attributes;
using InoAuthentification.Entities;
using InoAuthentification.UserManager.Models;
using InoAuthentification.UserManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppTestAuthentificationExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ExampleDbContext _context;

        public AccountController(ExampleDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("create")]
        [InovaAuthorized("AccountController")]
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
            catch (Exception e)
            {
                return new BadRequestResult();
            }
            return CreatedAtAction("CreatUser", new { createdUser.Id }, createdUser);
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
            TokenModel tokenModel;
            try
            {
                tokenModel = userManager.Login(loginModel);
            }
            catch (Exception e)
            {
                return new BadRequestResult();
            }
            return CreatedAtAction("Login", new { tokenModel.User.Id }, tokenModel);

        }


    }
}