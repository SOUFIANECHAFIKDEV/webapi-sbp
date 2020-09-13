using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class HistoriqueModel
    {
        public DateTime date { get; set; }
        public int IdUser { get; set; }
        public int action { get; set; }
        public List<ModifyEntryModel> champs { get; set; }
    }
}
