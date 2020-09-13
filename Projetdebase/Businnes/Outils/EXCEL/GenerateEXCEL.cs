using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.EXCEL
{
    public class GenerateEXCEL : IGenerateEXCEL
    {
        private readonly ProjetBaseContext _context;

        public GenerateEXCEL(ProjetBaseContext context)
        {
            _context = context;
        }

        public byte[] GenerateJournalAchatFile(IEnumerable<JournalVenteModel> datasource)
        {
            try
            {
                
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                        // Sets Headers
                        ws.Cells[1, 1].Value = "Code journal";
                        ws.Cells[1, 2].Value = "Date";
                        ws.Cells[1, 3].Value = "Numéro de compte";
                        ws.Cells[1, 4].Value = "Numéro de pièce";
                        ws.Cells[1, 5].Value = "Fournisseur";
                        ws.Cells[1, 6].Value = "Débit";
                        ws.Cells[1, 7].Value = "Crédit";

                        int i = 0;
                        foreach (var journalBanque in datasource)
                        {
                            ws.Cells[i + 2, 1].Value = journalBanque.CodeJournal;
                            ws.Cells[i + 2, 2].Value = journalBanque.DateCreation.ToString("dd/MM/yyyy");
                            ws.Cells[i + 2, 3].Value = journalBanque.NumeroCompte;
                            ws.Cells[i + 2, 4].Value = journalBanque.NumeroPiece;
                            ws.Cells[i + 2, 5].Value = journalBanque.NomClient;
                            ws.Cells[i + 2, 6].Value = journalBanque.Debit;
                            ws.Cells[i + 2, 7].Value = journalBanque.Credit;
                            i = i + 1;
                        }

                        // Format Header of Table
                        using (ExcelRange rng = ws.Cells["A1:G1"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(Color.Gold); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.AutoFitColumns();
                        }
                        //return pck;
                        var excelsBytes = excelPackage.GetAsByteArray();
                        return excelsBytes;
                    }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public byte[] GenerateJournalBanqueFile(IEnumerable<JournalBanqueModel> datasource)
        {
            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {

                    ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                    // Sets Headers
                    ws.Cells[1, 1].Value = "Code journal";
                    ws.Cells[1, 2].Value = "Date";
                    ws.Cells[1, 3].Value = "Numéro de compte";
                    ws.Cells[1, 4].Value = "Numéro de pièce";
                    ws.Cells[1, 5].Value = "Tiers";
                    ws.Cells[1, 6].Value = "Débit";
                    ws.Cells[1, 7].Value = "Crédit";
                    ws.Cells[1, 8].Value = "Type paiement";

                    int i = 0;
                    foreach (var journalBanque in datasource)
                    {
                        ws.Cells[i + 2, 1].Value = journalBanque.CodeJournal;
                        ws.Cells[i + 2, 2].Value = journalBanque.DatePaiement.ToString("dd/MM/yyyy");
                        ws.Cells[i + 2, 3].Value = journalBanque.NumeroCompte;
                        ws.Cells[i + 2, 4].Value = journalBanque.NumeroPiece;
                        ws.Cells[i + 2, 5].Value = journalBanque.Tiers;
                        ws.Cells[i + 2, 6].Value = journalBanque.Debit;
                        ws.Cells[i + 2, 7].Value = journalBanque.Credit;
                        ws.Cells[i + 2, 8].Value = journalBanque.TypePaiement;
                        i = i + 1;
                    }

                    // Format Header of Table
                    using (ExcelRange rng = ws.Cells["A1:H1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                        rng.Style.Fill.BackgroundColor.SetColor(Color.Gold); //Set color to DarkGray 
                        rng.Style.Font.Color.SetColor(Color.Black);
                        rng.AutoFitColumns();
                    }
                    //return pck;
                    var excelsBytes = excelPackage.GetAsByteArray();
                    return excelsBytes;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public byte[] GenerateJournalVenteFile(IEnumerable<JournalVenteModel> datasource)
        {
            try
            {
                
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                        // Sets Headers
                        ws.Cells[1, 1].Value = "Code journal";
                        ws.Cells[1, 2].Value = "Date";
                        ws.Cells[1, 3].Value = "Numéro de compte";
                        ws.Cells[1, 4].Value = "Numéro de pièce";
                        ws.Cells[1, 5].Value = "Client";
                        ws.Cells[1, 6].Value = "Débit";
                        ws.Cells[1, 7].Value = "Crédit";

                        int i = 0;
                        foreach (var journalBanque in datasource)
                        {
                            ws.Cells[i + 2, 1].Value = journalBanque.CodeJournal;
                            ws.Cells[i + 2, 2].Value = journalBanque.DateCreation.ToString("dd/MM/yyyy");
                            ws.Cells[i + 2, 3].Value = journalBanque.NumeroCompte;
                            ws.Cells[i + 2, 4].Value = journalBanque.NumeroPiece;
                            ws.Cells[i + 2, 5].Value = journalBanque.NomClient;
                            ws.Cells[i + 2, 6].Value = journalBanque.Debit;
                            ws.Cells[i + 2, 7].Value = journalBanque.Credit;
                            i = i + 1;
                        }

                        // Format Header of Table
                        using (ExcelRange rng = ws.Cells["A1:G1"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(Color.Gold); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.AutoFitColumns();
                        }
                        //return pck;
                        var excelsBytes = excelPackage.GetAsByteArray();
                        return excelsBytes;
                    }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
    }
}
