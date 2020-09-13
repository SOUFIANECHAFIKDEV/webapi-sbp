using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InoAuthentification.Entities
{
    [Table("profile_action")]
    public partial class ProfileAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("IdprofileNavigation")]
        public int Idprofile { get; set; }

        [ForeignKey("IdactionNavigation")]
        public int Idaction { get; set; }

        public Action IdactionNavigation { get; set; }
        public Profile IdprofileNavigation { get; set; }
    }
}
