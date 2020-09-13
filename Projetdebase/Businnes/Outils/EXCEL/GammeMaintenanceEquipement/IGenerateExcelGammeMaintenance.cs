using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.EXCEL
{
    interface IGenerateExcelGammeMaintenance
    {
        //  byte[] GenerateJournalBanqueFile(IEnumerable<JournalBanqueModel> datasource);
        byte[] GenerateGammeMaintenenceEquipement(Entities.ContratEntretien contratEntretien);
       
    }
}
