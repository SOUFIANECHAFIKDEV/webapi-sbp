using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.Pdf_Devis
{
    interface IPdfdevis
    {
        byte[] GenerateDocument();
    }
}
