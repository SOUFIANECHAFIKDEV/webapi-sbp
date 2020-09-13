using System;
using System.Collections.Generic;

namespace AppTestAuthentificationExample.Entities
{
    public partial class Departement
    {
        public Departement()
        {
            Ville = new HashSet<Ville>();
        }

        public int Id { get; set; }
        public string DepartementCode { get; set; }
        public string DepartementNom { get; set; }
        public int? IdPays { get; set; }

        public ICollection<Ville> Ville { get; set; }
    }
}
