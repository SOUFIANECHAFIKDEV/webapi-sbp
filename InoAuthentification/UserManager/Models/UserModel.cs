using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.UserManager.Models
{
   public class UserModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public int? Actif { get; set; }
        public DateTime? Dernierecon { get; set; }
        public DateTime JoinDate { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public int Accessfailedcount { get; set; }
        public string Username { get; set; }
        public string Historique { get; internal set; }
        public int PasswordAlreadyChanged { get; set; }
        public int IdProfile { get; set; }
        public int? IdClient { get; set; }
    }
}
