using InoAuthentification.Entities;
using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Account
{
    interface IAccountRepository : IRepository<User, int>
    {
        bool CheckUniqueReference(int id);
        Task<bool> SendRecoverEmail(RecoverPasswordModel recoverPasswordModel, ProjetBase.Businnes.Entities.ConfigMessagerie configMessagerie);
        User CheckExistsEmail(string email);
        List<ModifyEntryModel> GetModification(User userInDB, User user);
        User GetUser(int id);
        List<ModifyEntryModel> ChangeHistoriqueAttributsNames(List<ModifyEntryModel> historiqueChamps);
        bool supprimerUser(int id);
        bool CheckUniqueUserName(string userName);
        InoAuthentification.Entities.User getUserClientInfos(int id);
    }
}
