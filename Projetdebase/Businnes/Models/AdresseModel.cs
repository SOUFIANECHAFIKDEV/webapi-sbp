using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class AdresseModel
    {
        public string designation { get; set; }
        public string adresse { get; set; }
        public string complementAdresse { get; set; }
        public string departement { get; set; }
        public string ville { get; set; }
        public string codePostal { get; set; }
        public pays pays { get; set; }
        public bool @default { get; set; }
        //public bool default { get; set; }
    }
    public class pays
    {
        public int? id { get; set; }
        public int? code { get; set; }
        public string nomEnGb { get; set; }
        public string nomFrFr { get; set; }
    }
}
