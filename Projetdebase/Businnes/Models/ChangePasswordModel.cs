using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class ChangePasswordModel
    {
        public int idUser { get; set; }
        public string password { get; set; }
    }
}
