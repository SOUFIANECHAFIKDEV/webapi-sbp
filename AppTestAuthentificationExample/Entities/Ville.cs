using System;
using System.Collections.Generic;

namespace AppTestAuthentificationExample.Entities
{
    public partial class Ville
    {
        public int Id { get; set; }
        public int? IdDepartement { get; set; }
        public string VilleNom { get; set; }
        public string VilleNomReel { get; set; }
        public string CodePostal { get; set; }

        public Departement IdDepartementNavigation { get; set; }
    }
}
