using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.Pdf_intervention
{
    interface IPdfIntervention
    {
        byte[] GenerateDocument();
    }
}
