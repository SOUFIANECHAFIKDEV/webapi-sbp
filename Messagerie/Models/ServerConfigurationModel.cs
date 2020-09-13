using System;
using System.Collections.Generic;
using System.Text;

namespace InoMessagerie.Models
{
    public class ServerConfigurationModel
    {
        public string username { get; set; }

        public string password { get; set; }

        public string server { get; set; }

        public int port { get; set; }

        public int useSsl { get; set; }
    }
}
