using System;
using System.Collections.Generic;

namespace AppTestAuthentificationExample.Entities
{
    public partial class Pays
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string NomEnGb { get; set; }
        public string NomFrFr { get; set; }
    }
}
