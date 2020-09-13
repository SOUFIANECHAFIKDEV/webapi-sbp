using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Client
{
    interface IClientRepository : IRepository<Entities.Client, int>
    {
        bool CheckUniqueCodeClient(string reference);
        Task<bool> saveMemos(int id, string memos);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);
        bool supprimerClient(int id);
        Task<List<Entities.Client>> GetAllClient();
        Task<bool> UpdateMemos(EditMemosViewModel body);
    }
}
