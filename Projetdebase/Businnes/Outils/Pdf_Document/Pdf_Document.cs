using iTextSharp.text;
using iTextSharp.text.pdf;

using Newtonsoft.Json;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using iTextSharp.text.xml;
using iTextSharp.text.html.simpleparser;


using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Repositories;

namespace ProjetBase.Businnes.Outils.Pdf_Document
{
    public class Pdf_Document : IPdf_Document
    {
       
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
        Font fontH12Bold = FontFactory.GetFont("Arial", 8, Font.BOLDITALIC, BaseColor.Black);

        Font fontH12 = FontFactory.GetFont("Arial", 9, Font.ITALIC, BaseColor.Black);
        Font fontH10Bold = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.Black);
        Font fontH13 = FontFactory.GetFont("Arial", 14, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontH14 = FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.Black);
        Font fontH15 = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.Black);
        Font fontH15White = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.White);
        Font fontH11G = FontFactory.GetFont("Arial", 8, Font.NORMAL, new BaseColor(161, 165, 169));
        Font fontHItalic = FontFactory.GetFont("Arial", 8, Font.ITALIC, BaseColor.Black);

        // Images
        Image logo = Image.GetInstance(Directory.GetCurrentDirectory() + @"/Ressources/images/sbp.png");

        private static PdfWriter writer;
        PdfWriter wri;
  
        ParametrageDocument param;
       
        public Pdf_Document()
        {  }
       
        private byte[] NotFound()
        {
            throw new NotImplementedException();
        }
   

        public Document CreateDocument()
        {
            return new Document(PageSize.Letter, 30, 30, 5, 60);
        }

        public void CloseDocument(Document doc)
        {
            if (doc != null)
                doc.Close();
        }

        public void OpenDocument(Document doc)
        {
            if (doc != null)
                doc.Open();
        }

        public byte[] GeneratePDFWriter(Document doc, MemoryStream memStream)
        {
            writer = PdfWriter.GetInstance(doc, memStream);
            writer.PageEvent = new Footer();
            return memStream.ToArray();
      
        }

        public void Header(Document document, dynamic _document, Parametrages ParametrageDocument)
        {
            try
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
                //info societe

                ParametrageDocument paramsFact = JsonConvert.DeserializeObject<ParametrageDocument>(ParametrageDocument.Contenu);
                if (_document.GetType() == typeof(Facture))
                {

                    var cc = CreateSimpleHtmlParagraph(paramsFact.entete_facture == null ? "" : (paramsFact.entete_facture));
                leftRowseco.AddCell(new PdfPCell(cc) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_MIDDLE });

                }
                else
                {
                    leftRowseco.AddCell(new PdfPCell(CreateSimpleHtmlParagraph(paramsFact.entete_avoir == null ? "" : (paramsFact.entete_avoir))) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_MIDDLE });

                }
                
                leftRow.AddCell(new PdfPCell(leftRowseco) { BorderWidth = 0 });

                leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                // Right table
                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;

                 rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                if (_document.GetType() == typeof(Facture))
                {
                    rightRow.AddCell(new PdfPCell(new Paragraph("Facture", fontH10Bold)) { PaddingLeft = 50f, BorderWidth = 0, BorderColor = borderBotomColor });
                }
                else if (_document.GetType() == typeof(Avoir))
                {
                    rightRow.AddCell(new PdfPCell(new Paragraph("Avoir", fontH10Bold)) { PaddingLeft = 50f, BorderWidth = 0, BorderColor = borderBotomColor });
                }
                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                //info client
                var codeClient = "";
                if (_document.IdChantier != null)
                {
                    codeClient = _document.Chantier.Client.Codeclient;
                }
                else
                {
                    codeClient = _document.Client.Codeclient;

                }

                PdfPTable infoClient = new PdfPTable(6);
                // HEADER table info client
                var avoir_reference = (_document.Reference == null) ? "" : _document.Reference;
                var facture_Datecreation = (_document.DateCreation == null) ? "" : _document.DateCreation.ToString("dd/MM/yyyy");

                if (_document.GetType() == typeof(Avoir))
                {
                    infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Avoir n° ", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                }
                else if (_document.GetType() == typeof(Facture))
                {
                    infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Facture n° ", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                }
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Date", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(avoir_reference, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f,  HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(facture_Datecreation, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f,  HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(codeClient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f,  HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });


                rightRow.AddCell(new PdfPCell(infoClient) { BorderWidth = 0, PaddingTop = 0 });

                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                //add adresse


                PdfPTable adresseRow = new PdfPTable(1);
                adresseRow.WidthPercentage = 80;
                adresseRow.DefaultCell.Border = Rectangle.NO_BORDER;
                //adresse facturation


                AdresseFacturation Adresse = null;
                if (_document.InfoClient != null)
                {
                    var InfoClient = JsonConvert.DeserializeObject<InfoClientModel>(_document.InfoClient);
                    AdresseFacturation adresses = InfoClient.adresseFacturation;
                    Adresse = adresses;
                }
                adresseRow.AddCell(new PdfPCell(new Paragraph(new Chunk("ADRESSE FACTURATION:", fontH10B))) { BorderWidth = 0 });


                if (Adresse.@default == true)
                {
                    adresseRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.adresse, fontH10))) { BorderWidth = 0 });
                    adresseRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.complementAdresse, fontH10))) { BorderWidth = 0 });
                    adresseRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.codePostal + " " + Adresse.ville, fontH10))) { BorderWidth = 0 });

                }
                rightRow.AddCell(new PdfPCell(adresseRow) { BorderWidth = 0, PaddingTop = 10 });


                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 0 });

                document.Add(table_header);
             
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
             
            }
        }

        public void BodyArticles(iTextSharp.text.Document document, dynamic _document)
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

                if(_document.Object != null && _document.Object != "")
                {
                    articlestop.AddCell(new PdfPCell(new Paragraph(new Chunk(_document.Object, fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                    prestations.AddCell(new PdfPCell(articlestop) { BorderWidth = 0, PaddingBottom = 10 });
                    document.Add(prestations);
                }
                

             
                PdfPTable articlestab = new PdfPTable(12)
                { SpacingBefore = 7f, WidthPercentage = 100 };
                articlestab.DefaultCell.Border = Rectangle.NO_BORDER;

                // Header table
                //  articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("N° ", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 1 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Article", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Désignation", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Quantité", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("P.U.H.T", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant H.T", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                PrestationsModule DataPres = null;
                if (_document.Prestations != null)
                {


                    List<PrestationsModule> Prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(_document.Prestations);
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
                            if (DataPres.type == (int)typePrestation.Lot || DataPrestations.lotProduits != null)
                            {

                                List<DataLotProduit> ProduitLot = DataPrestations.lotProduits;
                                var taille = ProduitLot.Count;
                                //articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 12 });
                                var totahHt = totalHtLot(ProduitLot);

                                // Header table Lot
                                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
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
                                        double result = ProduitLot[j].idProduitNavigation.prixHt;

                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", result * qte) + " €", fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });


                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, ex.Message);
                                    throw;
                                }
                             

                            }
                        }


                    }
                    document.Add(articlestab);
                    articlestab.DeleteBodyRows();

                }

            
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        public double totalHtLot(List<DataLotProduit> articles)
        {
            double cc = articles.Sum(x => (x.idProduitNavigation.qte) * x.idProduitNavigation.prixHt);
            return cc;

        }

        public void HeaderAdresee(iTextSharp.text.Document document, dynamic _document)
        {
            try
            {
               // document.Open();
                PdfPTable table_header = new PdfPTable(2);
                table_header.WidthPercentage = 100;

                // Left header
                PdfPTable leftRow = new PdfPTable(1);
              
                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;
                //adresse facturation


                AdresseFacturation Adresse = null;
                if (_document.InfoClient != null)
                {
                    var InfoClient = JsonConvert.DeserializeObject<InfoClientModel>(_document.InfoClient);
                    AdresseFacturation adresses = InfoClient.adresseFacturation;
                    Adresse = adresses;
                }
                rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk("ADRESSE FACTURATION:", fontH10B))) { BorderWidth = 0 });


                if (Adresse.@default == true)
                {
                    rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.adresse, fontH10))) {  BorderWidth = 0 });
                     rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.complementAdresse, fontH10))) {  BorderWidth = 0 });
                    rightRow.AddCell(new PdfPCell(new Paragraph(new Chunk((Adresse == null) ? " " : Adresse.codePostal + " " + Adresse.ville, fontH10))) { BorderWidth = 0 });

                }
                table_header.AddCell(new PdfPCell(rightRow) { BorderWidth = 0, PaddingTop = 10 });

                document.Add(table_header);
   
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                throw;
            }
        }

        public void calculTva(iTextSharp.text.Document document, dynamic _document)
        {
            try
            {   PdfPTable tabtva = new PdfPTable(3) { SpacingBefore = 5f, WidthPercentage = 100 };

                tabtva.DefaultCell.Border = Rectangle.NO_BORDER;
                tabtva.SetWidths(new float[] { 350f, 70f, 550f });

                PdfPTable tab_calculeMultiTva = new PdfPTable(1);
                tab_calculeMultiTva.DefaultCell.Border = Rectangle.NO_BORDER;
                tab_calculeMultiTva.WidthPercentage = 100;
                PdfPTable eclatementTVA = new PdfPTable(6);
                // HEADER
                //TvaModel

                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("TVA", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("Base H.T", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("Montant", fontH15))) { BorderWidth = 0, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                if(_document.Tva != "[]" && _document.Tva != "null" && _document.Tva != null)
                {
                    List<TvaModel> ListTva = JsonConvert.DeserializeObject<List<TvaModel>>(_document.Tva);

                    foreach (var item in ListTva)
                    {

                        eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("" + string.Format("{0:0.00}%", item.tva), fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", item.totalHT) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                        eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", item.totalTVA) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    }

                }
                else
                {

                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk("" + string.Format("{0:0.00}%", 0), fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", 0) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    eclatementTVA.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", 0) + " €", fontH15))) { BorderWidth = 0, Padding = 5f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                }

                tab_calculeMultiTva.AddCell(new PdfPCell(eclatementTVA) { BorderWidth = 0, BorderWidthBottom = 0.3f, BorderColorBottom = borderBotomColor });

                
                tab_calculeMultiTva.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                tabtva.AddCell(new PdfPCell(tab_calculeMultiTva) { BorderWidth = 0 });
                // space between tabs 
                tabtva.AddCell(new PdfPCell() { BorderWidth = 0 });
                //
                PdfPTable condition = new PdfPTable(2);
                //condition.WidthPercentage = 100;
                condition.DefaultCell.Border = Rectangle.NO_BORDER;
                condition.SetWidths(new float[] { 450f, 150f });
                if (_document.GetType() == typeof(Facture))
                {
                  
                    if ( _document.typeFacture == (int) TypeFacture.Situation)
                    {
                        List<FactureSituationDevis> ListFactureSituatione = JsonConvert.DeserializeObject<List<FactureSituationDevis>>(_document.Devis.Situation);
                        var FactureSituation = ListFactureSituatione.Where(f => f.idFacture == _document.Id).FirstOrDefault();
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Situation Cumullée H.T", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0,BorderWidthBottom=0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureSituation.situationCumulleeHT) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, BorderWidth = 0, VerticalAlignment = Element.ALIGN_BOTTOM });

                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Situation Cumullée T.T.C", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureSituation.situationCumulleeTTC) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });


                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Acompte Cumullée H.C", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureSituation.acomptesCumulleeHT) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Acompte Cumullée T.T.C", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureSituation.acomptesCumulleeTTC) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                    }

                    if (_document.typeFacture == (int)TypeFacture.Acompte)
                    {
                        List<FactureAcompteDevis> ListFactureAcomptes = JsonConvert.DeserializeObject<List<FactureAcompteDevis>>(_document.Devis.Acomptes);
                        var FactureAcompte = ListFactureAcomptes.Where(f => f.idFacture == _document.Id).FirstOrDefault();
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Acompte Cumullée H.C", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureAcompte.acomptesCumulleeHT) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Situation Cumullée T.T.C", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureAcompte.acomptesCumulleeTTC) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, BorderWidth = 0, VerticalAlignment = Element.ALIGN_BOTTOM });

                    }

                }
                double totalGenarale = 0;
                double Total_ht = _document.TotalHt * (1 + (_document.Prorata / 100));
                if (_document.TypeRemise == "%")
                {
                    totalGenarale = ((-100) * Total_ht) / (_document.Remise - 100);
                }
                else
                {
                    totalGenarale = Total_ht + _document.Remise;
                }
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("TOTAL GENERAL HT ", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", totalGenarale) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                // double Total_ht = devis.TotalHt * (1 + (devis.Prorata / 100));
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Remise globale HT", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                if (_document.TypeRemise == "%")
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", _document.Remise) + " %", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                }
                else
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", _document.Remise) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                }

                //condition.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) /*{ Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BorderWidth = 0 }*/);
                if (_document.Prorata != 0)
                {
                     condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n TOTAL GENERAL HT  (prorata" + _document.Prorata + "% inclus ):", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", Total_ht) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
               
                }
               double Montantht = _document.TotalHt / (1 + (_document.Prorata / 100));
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Montant H.T ", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", _document.TotalHt) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                // condition.AddCell(new PdfPCell(new Paragraph(new Chunk("", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BorderWidth = 0 });
                if (_document.Prorata != 0)
                {
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Part Prorata ", fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", (Total_ht - _document.TotalHt)) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderColorBottom = borderBotomColor, BorderWidthBottom = 0.75f, VerticalAlignment = Element.ALIGN_BOTTOM });

                }
                if (_document.Tva != "[]" && _document.Tva != "null" && _document.Tva != null)
                {
                    List<TvaModel> ListTvaCollecte = JsonConvert.DeserializeObject<List<TvaModel>>(_document.Tva);

                    foreach (var item in ListTvaCollecte)
                    {

                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n T.V.A " + string.Format("{0:0.00}%", item.tva), fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", item.totalTVA) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    }
                }
                    

                double MontanthtMontantht = (_document.TotalHt * (_document.PUC / 100));
                Phrase phrasep = new Phrase();
                if(_document.PUC != 0)
                {
                    phrasep.Add(new Chunk("\n PARTICIPATION PUC" + _document.PUC + " % calculé sur", fontH15));
                    phrasep.Add(new Chunk("\n H.T(Prorata non compris) (H.T -" + _document.PUC + "% * " + _document.PUC / 100 + ")", fontH15));
                    condition.AddCell(new PdfPCell(phrasep) { Padding = 3f, BorderWidthLeft = 0, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", MontanthtMontantht) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });

                }


                if (_document.GetType() == typeof(Facture))
                {
                    if (_document.typeFacture == (int)TypeFacture.Situation )
                    {
                        
                        List<FactureSituationDevis> ListFactureSituatione = JsonConvert.DeserializeObject<List<FactureSituationDevis>>(_document.Devis.Situation);
                        var FactureSituation = ListFactureSituatione.Where(f => f.idFacture == _document.Id).FirstOrDefault();

                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Reste à payer de devis " + _document.Devis.Reference, fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureSituation.resteAPayer) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    }
                    if ( _document.typeFacture == (int)TypeFacture.Acompte)
                    {
                        List<FactureAcompteDevis> ListFactureAcomptes = JsonConvert.DeserializeObject<List<FactureAcompteDevis>>(_document.Devis.Acomptes);
                        var FactureAcompte = ListFactureAcomptes.Where(f => f.idFacture == _document.Id).FirstOrDefault();
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Reste à payer de devis " + _document.Devis.Reference, fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                        condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", FactureAcompte.resteAPayer) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderWidthBottom = 0.75f, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    }
                  

                }
                if (_document.GetType() == typeof(Facture))
                {
                    Facture facture = _document;
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk("\n Montant réglement ", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 0, BorderWidthBottom = 0.75f, BorderWidth = 0, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_BOTTOM });
                    condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", _document.FacturePaiements == null ? 0 : facture.FacturePaiements.Sum(x => x.Montant)) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth = 0, BorderColorBottom = borderBotomColor, BorderWidthBottom = 0.75f, VerticalAlignment = Element.ALIGN_BOTTOM });

                }
                condition.SpacingAfter = 5;
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(" TOTAL GENERAL TTC ", fontH15))) { Padding = 3f, BorderWidthLeft = 0, BorderWidth = 0, BackgroundColor = tableHeaderColor, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_CENTER,PaddingTop=12f });
                condition.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:0.00}", _document.Total) + " €", fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0, BorderWidth = 0, BackgroundColor = tableHeaderColor, BorderColorBottom = borderBotomColor , VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 12f });


              


                tabtva.AddCell(new PdfPCell(condition) { BorderWidth = 0, });
               
                document.Add(tabtva);
              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                throw;
            }
        }

        public void conditionREG(iTextSharp.text.Document document, dynamic _document, Parametrages ParametrageDocument)
        {
            try
            {
               
                PdfPTable conditiontab = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 };
                conditiontab.SpacingAfter = 5f;
                conditiontab.WidthPercentage = 100;
                Phrase phrase = new Phrase();
                var diffirentdate = (_document.DateEcheance - _document.DateCreation).TotalDays;
                var avoir_dateecheance = (_document.DateEcheance == null) ? "" : _document.DateEcheance.ToString("dd/MM/yyyy");

                //conditiontab.AddCell(new PdfPCell(new Paragraph(new Chunk("Délais de paiement" + diffirentdate + " jours , soit le : " + avoir_dateecheance, _document.Total) ), fontH15))) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidthLeft = 0, BorderWidth = 0, BackgroundColor = tableHeaderColor, BorderColorBottom = borderBotomColor, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 12f });
                conditiontab.AddCell(new PdfPCell(new Paragraph(new Chunk(" Délais de paiement " + diffirentdate + " jours, soit le : " + avoir_dateecheance, fontH15))) { BorderWidthLeft = 0, BorderWidth = 0, VerticalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0 });

               // phrase.Add(new Chunk("Conditions de réglements : ", fontH12Bold));
                if (_document.GetType() == typeof(Facture))
                {
                    var htmlText = _document.ConditionRegelement != null ? _document.ConditionRegelement : (param.conditions_facture == null ? "" : ((param.conditions_facture)));
                    phrase.Add(CreateSimpleHtmlParagraph(htmlText));
                }
                else if (_document.GetType() == typeof(Avoir))
                {
                    var htmlText = _document.ConditionRegelement != null ? _document.ConditionRegelement : (param.conditions_avoir == null ? "" : ((param.conditions_avoir)));
                    phrase.Add(CreateSimpleHtmlParagraph(htmlText));
                }
                conditiontab.AddCell(new PdfPCell(phrase) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0 });

               // Phrase phraseDateEcheance = new Phrase();
               // phraseDateEcheance.Add(new Chunk("Date d'échéance : ", fontH12Bold));
              //  phraseDateEcheance.Add(new Chunk(avoir_dateecheance, fontH10));
                //conditiontab.AddCell(new PdfPCell(phraseDateEcheance) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0 });

                document.Add(conditiontab);
               

            }
            catch (Exception ex)
            {  throw ex;
            }
        }

        
        public void PiedPage(iTextSharp.text.Document document, dynamic _doc, Parametrages ParametrageDocument)
        {
            try
            {
                PdfPTable piedtab = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 }; ;
                piedtab.SpacingAfter = 5f;
                PdfPTable pdfTab = new PdfPTable(1);
                //pdfTab.HorizontalAlignment = Element.ALIGN_BOTTOM;
                //pdfTab.TotalWidth = 550; pdfTab.AddCell(new PdfPCell() { BorderWidth = 0, FixedHeight = 10f });

                PdfPTable piedContenu = new PdfPTable(1);
       
                ParametrageDocument paramDoc = JsonConvert.DeserializeObject<ParametrageDocument>(ParametrageDocument.Contenu);

              //  piedContenu.SetWidths(new float[] { 55f, 60f });
                if (_doc.GetType() == typeof(Facture))
                {
                    piedContenu.AddCell(new PdfPCell(CreateSimpleHtmlParagraph((_doc.Note != null) ? _doc.Note : (paramDoc.note_facture == null ? "" : (paramDoc.note_facture)))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                }
                else if (_doc.GetType() == typeof(Avoir))
                {
                    piedContenu.AddCell(new PdfPCell(CreateSimpleHtmlParagraph((_doc.Note != null) ? _doc.Note : (paramDoc.note_avoir == null ? "" : (paramDoc.note_avoir)))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                }
                piedContenu.AddCell(new PdfPCell(new Paragraph()) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_BOTTOM, PaddingRight = 15 });
                pdfTab.AddCell(new PdfPCell(piedContenu) { BorderWidth = 0 });

               // pdfTab.WriteSelectedRows(0, -1, 50, 130, writer.DirectContent);
                piedtab.AddCell(new PdfPCell(pdfTab) { BorderWidth = 0 });
                document.Add(piedtab);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          public partial class Footer : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter wri, iTextSharp.text.Document doc)
            {
                PdfTemplate templateNumPage;

                templateNumPage = wri.DirectContent.CreateTemplate(30, 40);
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
               
                footerPied.AddCell(new PdfPCell() { FixedHeight = 1f, Padding = 0.5f, BorderWidthBottom = 0, BorderWidthTop = 0.5f, BorderWidth = 0, BorderColorTop = borderTopColor, PaddingTop = 10 });

                pdfTab.AddCell(new PdfPCell(footerPied) { BorderWidth = 0, PaddingLeft = 40, PaddingRight = 40 });
                pdfTab.AddCell(new PdfPCell(new Paragraph("www.cvc-sbp.fr", fontH13)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                pdfTab.WriteSelectedRows(0, -2, 18, 60, wri.DirectContent);


                footerTab.AddCell(new PdfPCell(pdfTab) { BorderWidth = 0 });

            }
        }

        private Paragraph CreateSimpleHtmlParagraph(string text)
        {
            try
            {
                //Our return object
                Paragraph p = new Paragraph();

                //ParseToList requires a StreamReader instead of just text
                using (var sr = new StringReader(text))
                {
                    var elements = HtmlWorker.ParseToList(sr, null);
                    foreach (var e in elements)
                    {
                        p.Add(e);
                    }
                }
                return p;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }



    }
}
