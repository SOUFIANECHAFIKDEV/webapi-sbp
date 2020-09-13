using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.EXCEL
{
    interface IGenerateEXCEL
    {
        byte[] GenerateJournalBanqueFile(IEnumerable<JournalBanqueModel> datasource);
        byte[] GenerateJournalVenteFile(IEnumerable<JournalVenteModel> datasource);
        byte[] GenerateJournalAchatFile(IEnumerable<JournalVenteModel> datasource);
    }
}
