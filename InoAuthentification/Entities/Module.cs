using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InoAuthentification.Entities
{
    [Table("module")]
    public partial class Module
    {
        public Module()
        {
            Action = new HashSet<Action>();
        }

        [Key]
        public int Id { get; set; }
        public string Libelle { get; set; }

        public ICollection<Action> Action { get; set; }
    }
}
