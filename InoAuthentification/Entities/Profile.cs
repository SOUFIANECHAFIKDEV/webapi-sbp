using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InoAuthentification.Entities
{
    [Table("profile")]
    public partial class Profile
    {
        public Profile()
        {
            ProfileAction = new HashSet<ProfileAction>();
            UserProfile = new HashSet<UserProfile>();
        }

        [Key]
        public int Id { get; set; }
        public string Libelle { get; set; }

        public ICollection<UserProfile> UserProfile { get; set; }
        public ICollection<ProfileAction> ProfileAction { get; set; }
    }
}
