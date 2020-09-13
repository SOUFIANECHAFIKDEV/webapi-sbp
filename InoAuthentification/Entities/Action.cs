using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InoAuthentification.Entities
{
    [Table("action")]
    public partial class Action
    {
        public Action()
        {
            ProfileAction = new HashSet<ProfileAction>();
        }

        [Key]
        public int Id { get; set; }
        public string Libelle { get; set; }

        [Column("code_name")]
        public string CodeName { get; set; }

        [Column("id_module")]
        [ForeignKey("IdModuleNavigation")]
        public int IdModule { get; set; }

        
        public Module IdModuleNavigation { get; set; }
        public ICollection<ProfileAction> ProfileAction { get; set; }
    }
}
