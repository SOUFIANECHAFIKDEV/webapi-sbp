using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories;
using Serilog;

namespace ProjetBase.Businnes.Outils.PDF_FactureParPeriod
{
    public class PDFFactureParPeriod : IPDFFactureParPeriod
    {
        // Document
    
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

        // Images
        Image logo = Image.GetInstance(Directory.GetCurrentDirectory() + @"/Ressources/images/sbp.png");


        PdfWriter wri;
        Facture facture;
        ParametrageDocument param;
        private PdfWriter writer;

        public PDFFactureParPeriod()
        {
            
        }

        private string formatDateString(DateTime? myDate)
        {
            DateTime moment = System.Convert.ToDateTime(myDate);
            string date = moment.Day < 10 ? ("0" + moment.Day) : Convert.ToString(moment.Day);
            string month = moment.Month < 10 ? ("0" + moment.Month) : Convert.ToString(moment.Month);
            return date + "-" + month + "-" + moment.Year;
        }
        private byte[] NotFound()
        {
            throw new NotImplementedException();
        }
        public byte[] GeneratePDFWriter(Document doc, MemoryStream memStream)
        {
            writer = PdfWriter.GetInstance(doc, memStream);
            return memStream.ToArray();
        }

        public Document CreateDocument()
        {
            return new Document(PageSize.Letter, 20, 20, 42, 20);
        }

        public void OpenDocument(Document doc)
        {
            if (doc != null)
                doc.Open();
        }
        public void Header(iTextSharp.text.Document document, Client client, ExportFactureEtAvoirParPeriodModel paramsBody, Parametrages param, int typeDoc)
        {
            document.Open();

            PdfPTable table_header = new PdfPTable(2);
            table_header.WidthPercentage = 100;

            // Left header
            PdfPTable leftRow = new PdfPTable(1);
            leftRow.DefaultCell.Border = Rectangle.NO_BORDER;

            logo.ScaleAbsolute(130f, 50f);
            leftRow.AddCell(new PdfPCell(logo) { FixedHeight = 50f, BorderWidth = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT });
            leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
            PdfPTable leftRowseco = new PdfPTable(1);
            ParametrageDocument paramFacture;
            paramFacture = JsonConvert.DeserializeObject<ParametrageDocument>(param.Contenu);
            leftRow.AddCell(new PdfPCell(CreateSimpleHtmlParagraph(paramFacture.entete_facture == null ? "" : (paramFacture.entete_facture))) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_MIDDLE });

            //leftRowseco.AddCell(new PdfPCell(new Paragraph(paramFacture == null || paramFacture.entete_facture == null ? "" : (paramFacture.entete_facture), fontH8)) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            leftRow.AddCell(new PdfPCell(leftRowseco) { BorderWidth = 0 });

            leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
            table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

            // Right table
            PdfPTable rightRow = new PdfPTable(1);
            rightRow.WidthPercentage = 80;
            rightRow.DefaultCell.Border = Rectangle.NO_BORDER;


            PdfPTable secondRight = new PdfPTable(1);
            rightRow.AddCell(new PdfPCell(secondRight) { BorderWidth = 0 });
            rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });

            // 
            if (typeDoc == 1)
            {
                rightRow.AddCell(new Paragraph("Relevé des Factures du " + formatDateString(paramsBody.DateDebut) + " au " + formatDateString(paramsBody.DateFin), fontH10Bold));
            }
            if (typeDoc == 2)
            {
                rightRow.AddCell(new Paragraph("Relance des Factures du " + formatDateString(paramsBody.DateDebut) + " au " + formatDateString(paramsBody.DateFin), fontH10Bold));
            }
            rightRow.AddCell(new PdfPCell() { FixedHeight = 10f, BorderWidth = 0 });
            PdfPTable infClient = new PdfPTable(1) { SpacingBefore = 4f };

            //InfoClient
           PdfPTable infoClient = new PdfPTable(6);
            //Adresse Client
            AdresseModel Adresse = null;
            if (client.Adresses != null && client.Adresses != "[]")
            {
                List<AdresseModel> adresses = JsonConvert.DeserializeObject<List<AdresseModel>>(client.Adresses);
                Adresse = adresses.FirstOrDefault();
            }

            //infClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Adresse Client : ", fontH10))) { BorderWidth = 0 });

            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.adresse, fontH10))) { BorderWidth = 0 });
            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.complementAdresse, fontH10))) { BorderWidth = 0 });
            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.codePostal + " " + Adresse.ville, fontH10))) { BorderWidth = 0 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Nom Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Adresse Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(client.Nom, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(client.Codeclient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            //infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(codeClient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(infClient) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
           


            //PdfPTable secondTableRight = new PdfPTable(1);
            //secondTableRight.AddCell(new PdfPCell(infoClient) { BorderWidth = 0 });
            rightRow.AddCell(new PdfPCell(infoClient) { BorderWidth = 0 });

            rightRow.AddCell(new PdfPCell() { FixedHeight = 10f, BorderWidth = 0 });
            table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 0 });



            document.Add(table_header);
            writer.PageEvent = new Footer();
        }

        private Phrase CreateSimpleHtmlParagraph(string text)
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

        public void BodyArticles(iTextSharp.text.Document document, List<Facture> Factures, List<Avoir> avoirs)
        {
            try
            {
                PdfPTable articlestab = new PdfPTable(8) { SpacingBefore = 5f, WidthPercentage = 100 };
                articlestab.DefaultCell.Border = Rectangle.NO_BORDER;

                // Header table
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Date", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Nature", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Référence interne", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant (€)", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Reste à payer (€)", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                var loopLength = Factures.Count();
                double? MONTANT = 0;
                double? sommePaye = 0;
                if (loopLength != 0)
                {
                    for (int i = 0; i < loopLength; i++)
                    {
                       
                        if(Factures[i].FacturePaiements != null) {
                            MONTANT = MONTANT + Factures[i].FacturePaiements.Sum(s => s.Montant);
                            sommePaye = Factures[i].FacturePaiements.Sum(s => s.Montant);
                        }

                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString(formatDateString(Factures[i].DateCreation)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString("Facture client"), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Factures[i].Reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00 €}", Factures[i].Total), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00 €}", (Factures[i].Total - sommePaye)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    }
                }
                loopLength = avoirs.Count();
                if (loopLength != 0)
                {
                    for (int i = 0; i < loopLength; i++)
                    {

                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString(formatDateString(avoirs[i].DateCreation)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString("Avoir client"), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(avoirs[i].Reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00} €", avoirs[i].Total), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    }
                }

                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("MONTANT", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, PaddingTop = 30f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 7 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00} €", MONTANT), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, PaddingTop = 30f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                document.Add(articlestab);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
            
        }

        public void BodyArticlesRelence(iTextSharp.text.Document document, List<Facture> Factures, List<Avoir> avoirs)
        {
            try
            {
                PdfPTable articlestab = new PdfPTable(7) { SpacingBefore = 7f, WidthPercentage = 100 };
                articlestab.DefaultCell.Border = Rectangle.NO_BORDER;

                // Header table
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Date", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Référence interne", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant (€)", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Reste à payer (€)", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                var loopLength = Factures.Count();
                double? MONTANT = 0;
                double? sommePaye = 0;
                if (loopLength != 0)
                {
                    for (int i = 0; i < loopLength; i++)
                    {

                        if (Factures[i].FacturePaiements != null)
                        {
                            MONTANT = MONTANT + Factures[i].FacturePaiements.Sum(s => s.Montant);
                            sommePaye = Factures[i].FacturePaiements.Sum(s => s.Montant);
                        }

                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString(formatDateString(Factures[i].DateCreation)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Factures[i].Reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00 €}", Factures[i].Total), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00 €}", (Factures[i].Total - sommePaye)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    }
                }
                loopLength = avoirs.Count();
                if (loopLength != 0)
                {
                    for (int i = 0; i < loopLength; i++)
                    {

                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Convert.ToString(formatDateString(avoirs[i].DateCreation)), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(avoirs[i].Reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00} €", avoirs[i].Total), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    }
                }

                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("MONTANT", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, PaddingTop = 30f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 7 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00} €", MONTANT), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, PaddingTop = 30f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                document.Add(articlestab);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        public void CloseDocument(iTextSharp.text.Document doc)
        {
            if (doc != null)
                doc.Close();
        }

      
     
        public void HeaderFactureRelance(iTextSharp.text.Document document, Client client, exportReleveRelanceFactureModel paramsBody, Parametrages param, int typeDoc)
        {
            document.Open();

            PdfPTable table_header = new PdfPTable(2);
            table_header.WidthPercentage = 100;

            // Left header
            PdfPTable leftRow = new PdfPTable(1);
            leftRow.DefaultCell.Border = Rectangle.NO_BORDER;

            logo.ScaleAbsolute(130f, 50f);
            leftRow.AddCell(new PdfPCell(logo) { FixedHeight = 50f, BorderWidth = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT });
            leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
            PdfPTable leftRowseco = new PdfPTable(1);
            ParametrageDocument paramFacture;
            paramFacture = JsonConvert.DeserializeObject<ParametrageDocument>(param.Contenu);
            // string ParametrageFactures = parametrageDocument.ParametrageFactures;
            // paramFacture = JsonConvert.DeserializeObject<parametrageFactureModel>(ParametrageFactures);
            leftRowseco.AddCell(new PdfPCell(CreateSimpleHtmlParagraph(paramFacture.entete_facture == null ? "" : (paramFacture.entete_facture))) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_MIDDLE });

            leftRow.AddCell(new PdfPCell(leftRowseco) { BorderWidth = 0 });

            leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
            table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

            // Right table
            PdfPTable rightRow = new PdfPTable(1);
            rightRow.WidthPercentage = 80;
            rightRow.DefaultCell.Border = Rectangle.NO_BORDER;


            PdfPTable secondRight = new PdfPTable(1);
            rightRow.AddCell(new PdfPCell(secondRight) { BorderWidth = 0 });
            rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });

            rightRow.AddCell(new Paragraph("Relance des factures", fontH10Bold));

            rightRow.AddCell(new PdfPCell() { FixedHeight = 10f, BorderWidth = 0 });
            PdfPTable infClient = new PdfPTable(1) { SpacingBefore = 4f };

            PdfPTable infoClient = new PdfPTable(6);
            //Adresse Client
            AdresseModel Adresse = null;
            if (client.Adresses != null && client.Adresses != "[]")
            {
                List<AdresseModel> adresses = JsonConvert.DeserializeObject<List<AdresseModel>>(client.Adresses);
                Adresse = adresses.FirstOrDefault();
            }

            //infClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Adresse Client : ", fontH10))) { BorderWidth = 0 });

            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.adresse, fontH10))) { BorderWidth = 0 });
            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.complementAdresse, fontH10))) { BorderWidth = 0 });
            infClient.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.codePostal + " " + Adresse.ville, fontH10))) { BorderWidth = 0 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Nom Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Adresse Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(client.Nom, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(client.Codeclient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            //infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(codeClient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
            infoClient.AddCell(new PdfPCell(infClient) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });



            //PdfPTable secondTableRight = new PdfPTable(1);
            //secondTableRight.AddCell(new PdfPCell(infoClient) { BorderWidth = 0 });
            rightRow.AddCell(new PdfPCell(infoClient) { BorderWidth = 0 });

            rightRow.AddCell(new PdfPCell() { FixedHeight = 10f, BorderWidth = 0 });
            table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 0 });



            document.Add(table_header);
            writer.PageEvent = new Footer();
        }
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
