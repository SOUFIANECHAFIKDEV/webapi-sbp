using System;
using System.Collections.Generic;
using System.Text;
using InoMessagerie.Models;
namespace InoMessagerie.Models
{
    public class SendMailParamsModel
    {
        public string Subject { get; set; }

        public string content { get; set; }

        public string path { get; set; }

        public List<string> messageTo { get; set; }

        public List<string> Bcc { get; set; }

        public List<string> Cc { get; set; }

        public List<Attachments> attachments { get; set; }
    }
}
