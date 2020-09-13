using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Avoir
{
    interface IAvoirRepository : IRepository<Entities.Avoir, int>
    {
        Task<bool> SaveMemos(int id, string memos);
 
        Entities.Avoir GetAvoir(int id);
       // Task<List<Entities.Avoir>> GetAvoirsClient(int IdClient, List<int> status, DateTime? DateDebut, DateTime? DateFin);
        Task<List<Entities.Avoir>> GetAvoirsClient(int IdClient, List<int> status, DateTime? DateDebut, DateTime? DateFin);
        bool CheckUniqueReference(string reference);
        List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps);

    }
}
