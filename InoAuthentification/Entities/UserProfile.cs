using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InoAuthentification.Entities
{
    [Table("user_profile")]
    public partial class UserProfile
    {
        [Key]
        [ForeignKey("IduserNavigation")]
        public int Iduser { get; set; }
        [ForeignKey("IdprofileNavigation")]
        public int Idprofile { get; set; }

        public virtual Profile IdprofileNavigation { get; set; }
        public virtual User IduserNavigation { get; set; }
    }
}