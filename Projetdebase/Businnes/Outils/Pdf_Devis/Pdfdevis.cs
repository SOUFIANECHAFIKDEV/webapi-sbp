
using Newtonsoft.Json;

using Serilog;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProjetBase.Businnes.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ProjetBase.Businnes.Models;
using System.Collections;
using iTextSharp.text.html.simpleparser;
using ProjetBase.Businnes.Enum;
//using System.Linq;
//using System.Text.RegularExpressions;
namespace ProjetBase.Businnes.Outils.Pdf_Devis
{
    public class Pdfdevis : IPdfdevis
    {

        // Document
     
        //Document doc = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
        Document doc = new Document(PageSize.A4, 30, 30, 5, 60);

        // Colors
        BaseColor tableHeaderColor = new BaseColor(199, 233, 250);
        BaseColor borderBotomColor = new BaseColor(224, 224, 224);
        BaseColor borderTopColor = new BaseColor(195, 224, 134);

        // Fonts
        Font fontH9 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 12, BaseColor.Black);
        Font fontH7 = FontFactory.GetFont(FontFactory.COURIER, 6);

        Font fontH8 = FontFactory.GetFont("Arial", 10, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontH11 = FontFactory.GetFont("Arial", 9, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontH10 = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.Black);
        Font fontH10B = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.Black);

        Font fontH12 = FontFactory.GetFont("Arial", 9, Font.ITALIC, BaseColor.Black);
        Font fontH12Bold = FontFactory.GetFont("Arial", 8, Font.BOLDITALIC, BaseColor.Black);
        Font fontH10Bold = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.Black);
        Font fontH13 = FontFactory.GetFont("Arial", 14, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontH14 = FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.Black);
        Font fontH15 = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.Black);
        Font fontH11G = FontFactory.GetFont("Arial", 8, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontHItalic = FontFactory.GetFont("Arial", 8, Font.ITALIC, BaseColor.Black);

        // Images
        Image logo = Image.GetInstance(Directory.GetCurrentDirectory() + @"/Ressources/images/sbp.png");
       

        PdfWriter wri;
        Devis devis;
        ParametrageDocument param;
        public Pdfdevis(Devis Devis ,Parametrages parametrage)
        {
            devis = Devis;
           param = JsonConvert.DeserializeObject<ParametrageDocument>(parametrage.Contenu);
        }
        public byte[] GenerateDocument()
        {
            try
            {
                MemoryStream memStream = new MemoryStream();

                wri = PdfWriter.GetInstance(doc, memStream);
                 wri.PageEvent = new Footer();

                doc.Open();
                doc.Add(Header());
                doc.Add(HeaderAdresse());
                BodyPrtestation();
                // doc.Add(BodyPrtestation());
                doc.Add(CalculeMultiTva());
                doc.Add(PiedPage());
                doc.Add(ConditionREG());
                doc.Close();
                return memStream.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return NotFound();
            }
        }
        private byte[] NotFound()
        {
            throw new NotImplementedException();
        }

        public PdfPTable Header()
        {
            try
            {
                PdfPTable table_header = new PdfPTable(2);
                table_header.WidthPercentage = 100;

                // Left header
                PdfPTable leftRow = new PdfPTable(1);
                leftRow.DefaultCell.Border = Rectangle.NO_BORDER;

                logo.ScaleAbsolute(130f, 50f);
                leftRow.AddCell(new PdfPCell(logo) { FixedHeight = 50f, BorderWidth = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT });
                leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                PdfPTable leftRowseco = new PdfPTable(1);
                //info societe
                leftRowseco.AddCell(new PdfPCell(CreateSimpleHtmlParagraph(param.entete  == null ? "" : (param.entete))) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_LEFT });
                leftRow.AddCell(new PdfPCell(leftRowseco) { BorderWidth = 0 });

                leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                // Right table
                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;


                PdfPTable secondRight = new PdfPTable(1);
                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                rightRow.AddCell(new PdfPCell(new Paragraph("DEVIS", fontH10Bold)) { PaddingLeft = 50f, BorderWidth = 0 });
                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                //info client
                //rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client : " + devis.Chantier.Client.Codeclient, fontH10B))) { PaddingLeft = 50f, BorderWidth = 0 });

                //rightRow.AddCell(new PdfPCell(new Paragraph("Nom Client :" + devis.Chantier.Client.Nom, fontH10B)) { PaddingLeft = 50f,BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                //rightRow.AddCell(new PdfPCell() { FixedHeight = 7f, BorderWidth = 0 });
                PdfPTable infoClient = new PdfPTable(6);
                var codeClient = devis.Chantier.Client.Codeclient;
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Devis n°  ", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Date", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(devis.Reference, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(devis.DateCreation.ToString("dd/MM/yyyy"), fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(codeClient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                rightRow.AddCell(new PdfPCell(infoClient) { BorderWidth = 0, PaddingTop = 0 });

                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 0 });


                return table_header;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }
        public PdfPTable HeaderAdresse()
        {
            try
            {

                PdfPTable table_header = new PdfPTable(2);
                table_header.WidthPercentage = 100;

                // Left header
                PdfPTable leftRow = new PdfPTable(1);
                //adresse INTERVENTION
                leftRow.DefaultCell.Border = Rectangle.NO_BORDER;
                AdresseModel AdresseIntervention = null;
                if (devis.AdresseIntervention != null)
                {
                    AdresseModel adressesi = JsonConvert.DeserializeObject<AdresseModel>(devis.AdresseIntervention);
                    AdresseIntervention = adressesi;
                }
                leftRow.AddCell(new Paragraph("ADRESSE INTERVENTION :", fontH10B));
                leftRow.AddCell(new PdfPCell(new Paragraph(new Chunk((AdresseIntervention == null) ? " " : AdresseIntervention.adresse, fontH10))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                leftRow.AddCell(new PdfPCell(new Paragraph(new Chunk((AdresseIntervention == null) ? " " : AdresseIntervention.complementAdresse, fontH10))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                leftRow.AddCell(new PdfPCell(new Paragraph(new Chunk((AdresseIntervention == null) ? " " : AdresseIntervention.codePostal + " " + AdresseIntervention.ville, fontH10))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;
                //adresse facturation


                AdresseModel Adresse = null;
                if (devis.Chantier.Client.Adresses != null)
                {
                    List<AdresseModel> adresses = JsonConvert.DeserializeObject<List<AdresseModel>>(devis.Chantier.Client.Adresses);
                    Adresse = adresses.FirstOrDefault();
                }
                rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk("ADRESSE FACTURATION:", fontH10B))) {  BorderWidth = 0 });


                if (Adresse.@default == true) {
                    rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.adresse, fontH10))) {  BorderWidth = 0 });
                    rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.complementAdresse, fontH10))) {  BorderWidth = 0 });
                    rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.codePostal + " " + Adresse.ville, fontH10))) { BorderWidth = 0 });

                }

               table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 10 });
                return table_header;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }

        }
        public PdfPTable BodyPrtestation()
        {
            try
            {
                PdfPTable prestations = new PdfPTable(1);
                prestations.WidthPercentage = 100;
                prestations.DefaultCell.Border = Rectangle.NO_BORDER;
                prestations.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });

                PdfPTable articlestop = new PdfPTable(1);
                articlestop.WidthPercentage = 100;
                articlestop.DefaultCell.Border = Rectangle.NO_BORDER;
                if (devis.Objet != null && devis.Objet != "")
                {
                    articlestop.AddCell(new PdfPCell(new Paragraph(new Chunk(devis.Objet, fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    prestations.AddCell(new PdfPCell(articlestop) { BorderWidth = 0, PaddingBottom = 10 });
                    doc.Add(prestations);
                }

                PdfPTable articlestab = new PdfPTable(12)
                { SpacingBefore = 5f, WidthPercentage = 100 };
                articlestab.DefaultCell.Border = Rectangle.NO_BORDER;

                // Header table
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Article", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Désignation", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Quantité", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("P.U.H.T", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant H.T", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });



                PrestationsModule DataPres = null;
                List<PrestationsModule> Prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(devis.Prestation);
                var length = Prestations.Count;
                
                for (int i = 0; i < length; i++)
                {
                    //int nbr =1;
                    // int s = nbr++,
                    DataPres = Prestations[i];
                    Data DataPrestations = DataPres.data;
                    if (DataPres.type == (int)typePrestation.Produit || DataPrestations.lotProduits == null)
                    {
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0}", Prestations[i].data.qte), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        double result = Prestations[i].data.prixHt;
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        long qtelot = Prestations[i].data.qte;
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result * Prestations[i].data.qte) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                    }
                    else
                    {
                        if (DataPres.type == (int)typePrestation.Produit || DataPrestations.lotProduits != null)
                        {

                            List<DataLotProduit> ProduitLot = DataPrestations.lotProduits;
                            var taille = ProduitLot.Count;
                            var totahHt = totalHtLot(ProduitLot);

                            // Header table Lot
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom , fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2});
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(" Sous Total HT: " + string.Format("{0:0.00}", totahHt), fontHItalic))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });


                            try
                            {


                                for (int j = 0; j < taille; j++)
                                {

                                    //PurpleIdProduitNavigation Produits = ProduitLot.Select(x => x.IdProduitNavigation).FirstOrDefault();
                                    articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("     " + ProduitLot[j].idProduitNavigation.reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                                    articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(ProduitLot[j].idProduitNavigation.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });

                                    long qte = ProduitLot[j].idProduitNavigation.qte;
                                    articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0}", ProduitLot[j].idProduitNavigation.qte), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                                    //  int result = Int32.Parse(ProduitLot[j].IdProduitNavigation.PrixHt);
                                    double result = ProduitLot[j].idProduitNavigation.prixHt;

                                    articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                                    articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result * qte) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });


                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, ex.Message);
                                return null;
                            }
                        }
                    }
   
                    doc.Add(articlestab);
                    articlestab.DeleteBodyRows();


                }
          
            
                prestations.DeleteBodyRows();


                return prestations;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }


        public double totalHtLot(List<DataLotProduit> articles)
        {
            double cc = articles.Sum(x => (x.idProduitNavigation.qte) * x.idProduitNavigation.prixHt);
            return cc ;

        }


        public PdfPTable CalculeMultiTva()
        {
            try
            {
                PdfPTable tabtva = new PdfPTable(3) { SpacingBefore = 5f, WidthPercentage = 100 };
         
                tabtva.DefaultCell.Border = Rectangle.NO_BORDER;
                tabtva.SetWidths(new float[] { 350f,70f, 550f });
          
                PdfPTable tab_calculeMultiTva = new PdfPTable(1);
                tab_calculeMultiTva.DefaultCell.Border = Rectangle.NO_BORDER;
                tab_calculeMultiTva.WidthPercentage = 100;
                PdfPTable eclatementTVA = new PdfPTable(6);
                // HEADER
                //TvaModel
               
                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("TVA", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("Base H.T", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                List<TvaModel> ListTva = JsonConvert.DeserializeObject<List<TvaModel>>(devis.Tva);
            
                foreach (var item in ListTva)
                {

                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("" + string.Format("{0:0.00}%", item.tva), fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", item.totalHT) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}",item.totalTVA) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                }

                tab_calculeMultiTva.AddCell(new PdfPCell(eclatementTVA) { BorderWidth = 0,BorderWidthBottom = 0.3f, BorderColorBottom = borderBotomColor });
                
                tab_calculeMultiTva.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                tabtva.AddCell(new PdfPCell(tab_calculeMultiTva) { BorderWidth = 0 });
                // space between tabs 
                 tabtva.AddCell(new PdfPCell() { BorderWidth = 0 });
                //
                PdfPTable condition = new PdfPTable(2);
             
                condition.DefaultCell.Border = Rectangle.NO_BORDER;
                condition.SetWidths(new float[] { 450f,  150f });
                double totalGenarale = 0;
                double Total_ht = devis.TotalHt * (1 + (devis.Prorata / 100));
                if (devis.TypeRemise == "%")
                {
                    totalGenarale = ((-100) * Total_ht)/ (devis.Remise - 100);
                }
                else
                {
                    totalGenarale = Total_ht + devis.Remise;
                }
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("TOTAL GENERAL HT ", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", totalGenarale) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                // double Total_ht = devis.TotalHt * (1 + (devis.Prorata / 100));
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Remise globale HT", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                if (devis.TypeRemise == "%")
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", devis.Remise) + " %", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                }
                else
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", devis.Remise) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                }

                if(devis.Prorata != 0)
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n TOTAL GENERAL HT  (prorata" + devis.Prorata + "% inclus ):", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", Total_ht) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                }
                double Montantht = devis.TotalHt / (1 + (devis.Prorata / 100));
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Montant H.T ", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", devis.TotalHt) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                if (devis.Prorata != 0)
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Part Prorata ", fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f,BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", (Total_ht - devis.TotalHt)) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                }

                List<TvaModel> ListTvaCollecte = JsonConvert.DeserializeObject<List<TvaModel>>(devis.Tva);
                foreach (var item in ListTvaCollecte)
                {

                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n T.V.A " + string.Format("{0:0.00}%", item.tva), fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", item.totalTVA) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                }

                double MontanthtMontantht = (devis.TotalHt * (devis.PUC / 100));
                Phrase phrasep = new Phrase();
                phrasep.Add(new Chunk("\n PARTICIPATION PUC" + devis.PUC + " % calculé sur", fontH15));
                phrasep.Add(new Chunk("\n H.T(Prorata non compris) (H.T -" + devis.PUC + "% * " + devis.PUC / 100 + ")", fontH15));
                if(devis.PUC != 0)
                {
                    condition.AddCell(new PdfPCell(phrasep) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", MontanthtMontantht) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                }

                //condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n TOTAL GENERAL TTC ", fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, VerticalAlignment = Element.ALIGN_BOTTOM });
                //condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", devis.Total) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.SpacingAfter = 10;
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("TOTAL GENERAL TTC ", fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BackgroundColor = tableHeaderColor, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 12f });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", devis.Total) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0, BorderWidth = 0, BackgroundColor = tableHeaderColor, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 12f });


                tabtva.AddCell(new PdfPCell(condition) { BorderWidth = 0, });
                return tabtva;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

     

        public PdfPTable ConditionREG()
        {
            PdfPTable conditiontab = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 };
            conditiontab.SpacingAfter = 5f;
            conditiontab.WidthPercentage = 100;
            //DATA
            
            Phrase phrase = new Phrase();
            //phrase.Add(new Chunk("Conditions de réglements : ", fontH12Bold ));
            var htmlText = devis.ConditionReglement != null ? devis.ConditionReglement : (param.conditions == null ? "" : ((param.conditions)));
            phrase.Add(CreateSimpleHtmlParagraph(htmlText));

            conditiontab.AddCell(new PdfPCell(phrase) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0 });
            return conditiontab;
        }
        public PdfPTable PiedPage()
        {
        
            PdfPTable piedtab = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 }; ;
            piedtab.SpacingAfter = 5f;
            PdfPTable pdfTab = new PdfPTable(1);
            
            pdfTab.AddCell(new PdfPCell(CreateSimpleHtmlParagraph((devis != null && devis.Note != null) ? devis.Note : (param.note == null ? "" : (param.note)))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });
           piedtab.AddCell(new PdfPCell(pdfTab) { BorderWidth = 0 });


            return piedtab;
        }
        private Paragraph CreateSimpleHtmlParagraph(string text)
        {
            //Our return object
            Paragraph p = new Paragraph();

            //ParseToList requires a StreamReader instead of just text
            using (StringReader sr = new StringReader(text))
            {
                //Parse and get a collection of elements
                var elements = HtmlWorker.ParseToList(sr, null);
                foreach (IElement e in elements)
                {
                    //Add those elements to the paragraph
                    p.Add(e);
                }
            }
            //Return the paragraph
            return p;
        }

        public partial class Footer : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter wri, iTextSharp.text.Document doc)
            {
                PdfTemplate templateNumPage;

                templateNumPage = wri.DirectContent.CreateTemplate(30, 60);


                base.OnEndPage(wri, doc);
                BaseColor borderTopColor = new BaseColor(224, 224, 224);


                Font fontH13 = FontFactory.GetFont("Arial", 14, Font.NORMAL, new BaseColor(161, 165, 169));
                Font fontH14 = FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.Black);
                int courantPageNumber = wri.CurrentPageNumber;
                String pageText = "Page " + courantPageNumber.ToString();
                PdfPTable footerTab = new PdfPTable(3);
                PdfPTable pdfTab = new PdfPTable(1);
                pdfTab.HorizontalAlignment = Element.ALIGN_BOTTOM;
                pdfTab.TotalWidth = 550;
                pdfTab.AddCell(new PdfPCell() { BorderWidth = 0, FixedHeight = 5f });

                PdfPTable footerContenu = new PdfPTable(2);
                footerContenu.SetWidths(new float[] { 65f, 35f });
                footerContenu.AddCell(new PdfPCell(new Paragraph()) { BorderWidth = 0, PaddingLeft = 15, PaddingTop = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                footerContenu.AddCell(new PdfPCell(new Paragraph(pageText, fontH14)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_BOTTOM, PaddingRight = 15 });

                pdfTab.AddCell(new PdfPCell(footerContenu) { BorderWidth = 0 });
                pdfTab.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });


                PdfPTable footerPied = new PdfPTable(1);
                  
                footerPied.AddCell(new PdfPCell() { FixedHeight = 1f, Padding = 3f, BorderWidthBottom = 0, BorderWidthTop = 0.5f, BorderWidth = 0, BorderColorTop = borderTopColor, PaddingTop = 10 });

                pdfTab.AddCell(new PdfPCell(footerPied) { BorderWidth = 0, PaddingLeft = 40, PaddingRight = 40 });
                pdfTab.AddCell(new PdfPCell(new Paragraph("www.cvc-sbp.fr", fontH13)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                pdfTab.WriteSelectedRows(0, -2, 18, 60, wri.DirectContent);


                footerTab.AddCell(new PdfPCell(pdfTab) { BorderWidth = 0 });

            }
        }
       


    }
}
