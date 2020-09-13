using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class FicheTechniqueModal
    {
        public class ficheTechniqueModal
        {
            public string commentaire { get; set; }
            public string date { get; set; }
            public int idUser { get; set; }
            public List<FTPieceJointes> pieceJointes { get; set; }
        }

        public class FTPieceJointes
        {
            public string file { get; set; }
            public string name { get; set; }
            public string orignalName { get; set; }
            public string type { get; set; }
        }
    }
}
