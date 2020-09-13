using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ProjetBase.Businnes.Outils.EXCEL
{
    public class GenerateExcelGammeMaintenance : IGenerateExcelGammeMaintenance
    {
        private readonly ProjetBaseContext _context;

        public GenerateExcelGammeMaintenance(ProjetBaseContext context)
        {
            _context = context;
        }

        public byte[] GenerateGammeMaintenenceEquipement(Entities.ContratEntretien contratEntretien)
        {
            try
            {

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    var listEquipement = contratEntretien.EquipementContrat;
                    foreach (var gammeMaintenanaceEquipement in listEquipement)
                    {
                        var nomEquipement = gammeMaintenanaceEquipement.Nom.ToString();
                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add(nomEquipement);//nom de sheets
                       //Width
                        ws.Column(1).Width = 50;
                        ws.Column(2).Width = 5;
                        ws.Column(3).Width = 5;
                        ws.Column(4).Width = 5;
                        ws.Column(5).Width = 5;
                        ws.Column(6).Width = 5;
                        ws.Column(7).Width = 5;
                        ws.Column(8).Width = 5;
                        ws.Column(9).Width = 5;
                        ws.Column(10).Width = 5;
                        ws.Column(11).Width = 5;
                        ws.Column(12).Width = 5;
                        ws.Column(13).Width = 5;
                        ws.Column(14).Width = 40;
                        //Height first Row
                        ws.Row(1).Height = 25;
                        //Merge
                        ws.Cells[3, 1, 4, 13].Merge = true;
                        ws.Cells["A1:M1"].Merge = true;
                        ws.Cells["A2:M2"].Merge = true;
                        ws.Cells["B6:M7"].Merge = true;
                        ws.Cells[6, 1, 8, 1].Merge = true;
                        ws.Cells["N6:N8"].Merge = true;

                        // Sets Headers
                        ws.Cells["A1:M1"].Value = "Gamme de maintenance préventive";
                        ws.Cells["A2:M2"].Value = "";
                        ws.Cells[3, 1, 4, 13].Value = nomEquipement;
                        ws.Cells["B6:M7"].Value = "Périodicité";
                        ws.Cells[6, 1, 8, 1].Value = "Libellé opération";
                        ws.Cells["B8"].Value = "J";
                        ws.Cells["C8"].Value = "F";
                        ws.Cells["D8"].Value = "M";
                        ws.Cells["E8"].Value = "A";
                        ws.Cells["F8"].Value = "M";
                        ws.Cells["G8"].Value = "J";
                        ws.Cells["H8"].Value = "J";
                        ws.Cells["I8"].Value = "A";
                        ws.Cells["J8"].Value = "S";
                        ws.Cells["K8"].Value = "O";
                        ws.Cells["L8"].Value = "N";
                        ws.Cells["M8"].Value = "D";
                      
                        Site site = null;
                        if (contratEntretien.Site != null)
                        {
                            Site siteClient = JsonConvert.DeserializeObject<Site>(contratEntretien.Site);
                            site = siteClient;
                        }
                     
                        ws.Cells["N1"].Value = contratEntretien.Client.Nom;
                        ws.Cells["N2"].Value = site.designation;
                        ws.Cells["N3"].Value = site.adresse;
                        //ws.Cells["N4"].Value = "";
                        //ws.Cells["N5"].Value = "";
                        ws.Cells["N6:N8"].Value = "Observations - Outillage spécifique - Pièces détachées";

                        int i = 0;
                        foreach (var LotEquipement in gammeMaintenanaceEquipement.Libelle)
                        {
                            var nbrOperation = LotEquipement.OperationsEquipement.Count();
                               //Cas Periodecite dans le libelle 
                            if (nbrOperation == 1 && LotEquipement.OperationsEquipement.Select(x=>x.Nom == LotEquipement.Nom).FirstOrDefault())
                            {
                                ws.Cells[i + 9, 1].Value = LotEquipement.Nom;
                                var ListMois = LotEquipement.OperationsEquipement.Select(x => x.Periodicite).FirstOrDefault();
                                //var vv = ListMois.ToList();
                                var mois = ListMois.Select(x => x.Mois);
                                ExcelColumn col = ws.Column(1);
                                // this will resize the width to 50
                                col.AutoFit();
                                col.Width = 50;
                                // this is just a style property, and doesn't actually execute any recalculations
                                col.Style.WrapText = true;
                                if (col.Width > 50)
                                {
                                    col.Width = 50;
                                }
                                using (ExcelRange rng = ws.Cells[i + 9, 1])
                                {
                                    // rng.AutoFitColumns(1);
                                    rng.Style.Font.Bold = true;
                                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                }
                                foreach (var m in mois)
                                {
                                    ws.Cells[i + 9, (m + 1)].Value = "x";
                                    using (ExcelRange rng = ws.Cells[i + 9, 2, i + 9, 13])
                                    {
                                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Left.Color.SetColor(Color.Black);
                                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                    }
                                }
                                //n10+i
                                using (ExcelRange rng = ws.Cells[i + 9, 14])
                                {
                                    //rng.Style.Font.Bold = true;
                                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                                }
                                i = i + 1;

                            }
                            //Cas Periodicite dans operations
                            else
                            {
                                ws.Cells[i + 9, 1, i + 9, 13].Merge = true;
                                ws.Cells[i + 9, 1, i + 9, 13].Value = LotEquipement.Nom;
                                //N9+i
                                using (ExcelRange rng = ws.Cells[i + 9, 1, i + 9, 13])
                                {
                                    rng.Style.Font.Bold = true;
                                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Top.Color.SetColor(Color.Black);

                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                }
                                using (ExcelRange rng = ws.Cells[i + 9, 14])
                                {
                                    //rng.Style.Font.Bold = true;
                                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                }
                                foreach (var OperationsEquipement in LotEquipement.OperationsEquipement)
                                {
                                    ws.Cells[i + 10, 1].Value = OperationsEquipement.Nom;
                                    var mois = OperationsEquipement.Periodicite.Select(x => x.Mois);

                                    ExcelColumn col = ws.Column(1);
                                    // this will resize the width to 50
                                    col.AutoFit();
                                    col.Width = 50;
                                    // this is just a style property, and doesn't actually execute any recalculations
                                    col.Style.WrapText = true;
                                    if (col.Width > 50)
                                    {
                                        col.Width = 50;
                                    }
                                    //n10+i
                                    using (ExcelRange rng = ws.Cells[i + 10, 14])
                                    {
                                        //rng.Style.Font.Bold = true;
                                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Top.Color.SetColor(Color.Black);
                                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Left.Color.SetColor(Color.Black);
                                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Right.Color.SetColor(Color.Black);
                                    }
                                    using (ExcelRange rng = ws.Cells[i + 10, 1])
                                    {
                                        // rng.AutoFitColumns(1);
                                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Left.Color.SetColor(Color.Black);
                                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                    }

                                    foreach (var m in mois)
                                    {
                                        ws.Cells[i + 10, (m + 1)].Value = "x";
                                        using (ExcelRange rng = ws.Cells[i + 10, 2, i + 10, 13])
                                        {
                                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                                        }
                                    }
                                    i = i + 1;
                                }
                                i = i + 1;
                            }
                            
                        }
                      Color DeepBlueHexCode = ColorTranslator.FromHtml("#065cb3");

                        // Format Header of Table
                        using (ExcelRange rng = ws.Cells["A1:M1"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Font.Size = 12;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);

                            
                        }
                        using (ExcelRange rng = ws.Cells["A2:M2"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Font.Size = 12;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }


                        using (ExcelRange rng = ws.Cells["A5:M5"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 

                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 

                        }
                        using (ExcelRange rng = ws.Cells["B6:M7"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Font.Size = 12;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Top.Color.SetColor(Color.Black);
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);

                        }
                        using (ExcelRange rng = ws.Cells["B8:M8"])
                        {

                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);

                        }

                        //A3-M13 A4 M13
                        using (ExcelRange rng = ws.Cells[3, 1, 4, 13])
                        {
                            // ws.Column(1).Width = 100;
                            rng.Style.Font.Bold = true;
                            rng.Style.Font.Size = 12;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);

                        }
                        //A6 -A8
                        using (ExcelRange rng = ws.Cells[6, 1, 8, 1])
                        {
                            // ws.Column(1).Width = 100;
                            rng.Style.Font.Bold = true;
                            rng.Style.Font.Size = 12;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DarkGray 
                            rng.Style.Font.Color.SetColor(Color.Black);
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Top.Color.SetColor(Color.Black);
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Right.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                        }
                        using (ExcelRange rng = ws.Cells["N1"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DeepBlueHexCode 
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Right.Color.SetColor(Color.Black);

                        }

                        using (ExcelRange rng = ws.Cells["N2:N4"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DeepBlueHexCode 
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Right.Color.SetColor(Color.Black);

                        }
                        using (ExcelRange rng = ws.Cells["N5"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DeepBlueHexCode 
                            rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Top.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Right.Color.SetColor(Color.Black);

                        }
                        using (ExcelRange rng = ws.Cells["N6:N8"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid 
                            rng.Style.Font.Size = 9;
                            rng.Style.Fill.BackgroundColor.SetColor(DeepBlueHexCode); //Set color to DeepBlueHexCode 
                            rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            rng.Style.Border.Top.Color.SetColor(Color.Black);
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Color.SetColor(Color.Black);
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Right.Color.SetColor(Color.Black);

                        }
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
