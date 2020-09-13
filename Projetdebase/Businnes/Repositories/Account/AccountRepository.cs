using InoAuthentification.Entities;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ProjetBase.Businnes.Extensions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using InoAuthentification.UserManagers;

namespace ProjetBase.Businnes.Repositories.Account
{
    public class BonCommandeFournisseurtRepository : EntityFrameworkRepository<User, int>, IAccountRepository
    {

        public ProjetBaseContext ProjetBaseContext;

        public BonCommandeFournisseurtRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            ProjetBaseContext = dbContext;
        }

        public User CheckExistsEmail(string email)
        {
            try
            {
                return ProjetBaseContext.User.Where(x => x.Email == email).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

        public bool CheckUniqueReference(int id)
        {
            var NbrReference = DbContext.User.Where(x => x.Id == id).Count();
            return NbrReference > 0;
        }

        public async Task<bool> SendRecoverEmail(RecoverPasswordModel recoverPasswordModel, Entities.ConfigMessagerie configMessagerie)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = configMessagerie.Username,
                        Password = configMessagerie.Password
                    };

                    client.Credentials = credential;
                    client.Host = configMessagerie.Serveur;
                    client.Port = configMessagerie.Port;
                    client.EnableSsl = configMessagerie.Ssl == (int)TypeSSL.enable;

                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.To.Add(new MailAddress(configMessagerie.Username));
                        emailMessage.From = new MailAddress(recoverPasswordModel.email);
                        emailMessage.Subject = recoverPasswordModel.subject;
                        emailMessage.Body = recoverPasswordModel.body;
                        client.Send(emailMessage);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public List<ModifyEntryModel> GetModification(User userInDB, User user)
        {
            var champsModify = EntityExtensions.GetModification(userInDB, user);

            string[] noComparar = { "Passwordhash", "JoinDate", "Historique" };

            List<ModifyEntryModel> listComparar = new List<ModifyEntryModel>();

            foreach (var champ in champsModify)
            {
                if (noComparar.Contains(champ.Attribute) == false)
                {
                    listComparar.Add(champ);
                }
            }
            return listComparar;
        }

        public User GetUser(int id)
        {
            try
            {
                return DbContext.User.Include("Societe").Where(u => u.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

        public List<ModifyEntryModel> ChangeHistoriqueAttributsNames(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();



                historiqueChamps.ForEach(h =>
                {
                    if (h.Attribute != "PasswordAlreadyChanged" && h.Attribute != "Dernierecon")
                    {
                        //if (h.Attribute == "Dernierecon")
                        //    h.Attribute = "Dérniere connexion";

                        if (h.Attribute == "Phonenumber")
                            h.Attribute = "Téléphone";

                        if (h.Attribute == "Actif")
                        {
                            h.After = h.After == "0" ? "Non" : "Oui";
                            h.Before = h.Before == "0" ? "Non" : "Oui";
                        }
                        NewhistoriqueChamps.Add(h);
                    }
                });
                return NewhistoriqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public bool supprimerUser(int id)
        {
            var userManager = new UserManager(DbContext);
            var currentUser = userManager.GetCurrentUser();

            if (DbContext.Clients.Where(c => c.IdAgent == id).Count() != 0)
            {
                return false;
            }
            var user = DbContext.User.SingleOrDefault(C => C.Id == id);
            if (user.Id != currentUser.Id)
            {
                DbContext.Remove(user);
                DbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool CheckUniqueUserName(string userName)
        {
            var NbrReference = DbContext.User.Where(x => x.Username == userName).Count();
            return NbrReference > 0;
        }

        public User getUserClientInfos(int id)
        {
            User user = DbContext.User.Where(U => U.IdClient == id).FirstOrDefault();
            return user;
        }
    }

}
