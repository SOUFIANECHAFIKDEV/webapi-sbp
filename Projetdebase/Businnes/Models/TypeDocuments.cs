using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class TypeDocuments
    {
        //id nom fixed
        public int id { get; set; }
        public string nom { get; set; }
        public bool isFixed { get; set; }
    }

    public class ChantierDocs
    {
        public int id { get; set; }
        public int type { get; set; }
        public string commentaire { get; set; }
        public string designation { get; set; }
        public PieceJointes pieceJointes { get; set; }
        public DateTime date_ajout { get; set; }
        public DateTime date_derniere_modification { get; set; }
        public int idUser { get; set; }
    }

    public class PieceJointes
    {
        public string name { get; set; }
        public string type { get; set; }
        public string orignalName { get; set; }
        public string file { get; set; }
    }
}
