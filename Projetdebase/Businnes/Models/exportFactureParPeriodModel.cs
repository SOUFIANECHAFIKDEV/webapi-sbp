using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class exportFactureParPeriodModel
    {
        public List<byte[]> FacturesParPeriod { get; set; }
        public List<byte[]> FacturesAndAvoirsDetail { get; set; }
    }
}
