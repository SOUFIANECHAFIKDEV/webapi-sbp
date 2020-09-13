using InoAuthentification.DbContexts;
using InoAuthentification.Entities;
using InoAuthentification.Helpers;
using InoAuthentification.JwtManagers;
using InoAuthentification.PasswordManagers;
using InoAuthentification.UserManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InoAuthentification.UserManagers
{
    public class UserManager
    {
        public readonly InoAuthentificationDbContext _context;
        IHttpContextAccessor _httpContextAccessor = null;
        public UserManager(InoAuthentificationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> CreateUserAsync(User user, string password)
        {
            var userManager = new UserManager(_context);
            var currentUser =  userManager.GetCurrentUser();
            var passwordManager = new PasswordManager();
            user.Passwordhash = passwordManager.CreatePasswordSalt(user.Password);
            user.JoinDate = DateTime.Now;
            user.PasswordAlreadyChanged = 0;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return new UserModel
            {
                Id = user.Id,
                Accessfailedcount = user.Accessfailedcount,
                Actif = user.Actif,
                Dernierecon = user.Dernierecon,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Nom = user.Nom,
                Phonenumber = user.Phonenumber,
                Prenom = user.Prenom,
                Username = user.Username,
                Historique = user.Historique,
                PasswordAlreadyChanged = user.PasswordAlreadyChanged,
                IdProfile = user.IdProfile
            };
        }

        public TokenModel Login(LoginModel loginModel)
        {
            if (!(_context.User.Where(u => u.Username == loginModel.UserName).Count() > 0))
                return null;
            User user = _context.User.FirstOrDefault(u => u.Username == loginModel.UserName);
            if (user == null)
                return null;
            PasswordManager passwordManager = new PasswordManager();
            if (!passwordManager.IsPasswordValid(loginModel.Password, user.Passwordhash))
                return null;
            JwtManager jwtManager = new JwtManager();
            var token = jwtManager.CreatToken(user.Id);
            return new TokenModel
            {
                Token = token,
                User = new UserModel
                {
                    Id = user.Id,
                    Accessfailedcount = user.Accessfailedcount,
                    Actif = user.Actif,
                    Dernierecon = user.Dernierecon,
                    Email = user.Email,
                    JoinDate = user.JoinDate,
                    Nom = user.Nom,
                    Phonenumber = user.Phonenumber,
                    Prenom = user.Prenom,
                    Username = user.Username,
                    Historique = user.Historique,
                    PasswordAlreadyChanged = user.PasswordAlreadyChanged,
                    IdProfile = user.IdProfile,
                    IdClient = user.IdClient,
                }
            };
        }

        public User GetCurrentUser()
        {
            StringValues authorizationToken;
            InovaHelper.HttpCurrent.Request.Headers.TryGetValue("Authorization", out authorizationToken);
            JwtManager jwtManager = new JwtManager();
            var result = jwtManager.VerifyingToken(authorizationToken);
            if (result.IsValid)
            {
                using (var dbContext = new InoAuthentificationDbContext(InovaHelper._dbContextOptions))
                {
                    var user = dbContext.User.Where(x => x.Id == result.CurrentToken.UserId).FirstOrDefault();
                    return user;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> changePassword(int idUser, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                try
                {
                    var user = _context.User.Where(u => u.Id == idUser).FirstOrDefault();
                    if (user == null)
                    {
                        return false;
                    }
                    //user.PasswordAlreadyChanged = (int)PasswordStatut.active;
                    var passwordManager = new PasswordManager();
                    var Password = passwordManager.CreatePasswordSalt(password);
                    user.Passwordhash = Password;
                    user.PasswordAlreadyChanged = 1;
                    _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    //_context.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }


            }

        }
        
        public async Task<UserModel> updateUserInfos(User user)
        {
            var userInDB = _context.User.Where(w => w.Id == user.Id).FirstOrDefault();

            userInDB.Username = user.Username;
            userInDB.Nom = user.Nom;
            userInDB.Prenom = user.Prenom;
            userInDB.Email = user.Email;
            userInDB.Matricule = user.Matricule;
            userInDB.Phonenumber = user.Phonenumber;
            userInDB.Actif = user.Actif;
            userInDB.IdProfile = user.IdProfile;
            userInDB.Historique = user.Historique;

            _context.Entry(userInDB).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        
            return new UserModel
            {
                Id = user.Id,
                Accessfailedcount = userInDB.Accessfailedcount,
                Actif = userInDB.Actif,
                Dernierecon = userInDB.Dernierecon,
                Email = userInDB.Email,
                JoinDate = userInDB.JoinDate,
                Nom = userInDB.Nom,
                Phonenumber = userInDB.Phonenumber,
                Prenom = userInDB.Prenom,
                Username = userInDB.Username,
                Historique = userInDB.Historique,
                IdProfile = userInDB.IdProfile
            };
        }

        public async Task<bool> ActiverUser(int id)
        {
            var user =  _context.User.Where(u => u.Id == id).FirstOrDefault();
            if (user != null)
            {
                user.Actif = 1;
                _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

    }
}
