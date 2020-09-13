using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Outils.Generic;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using iTextSharp.text.html.simpleparser;
using ProjetBase.Businnes.Enum;

namespace ProjetBase.Businnes.Outils.Pdf_intervention
{
    public class PdfIntervention : IPdfIntervention
    {
        // Document
         Document doc = new Document(PageSize.A4, 30, 30, 5, 60);
        // Colors
        BaseColor tableHeaderColor = new BaseColor(199, 233, 250);
        BaseColor borderBotomColor = new BaseColor(224, 224, 224);
       // BaseColor borderTopColor = new BaseColor(195, 224, 134);
        BaseColor borderTopColor = new BaseColor(224, 224, 224);

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
        FicheIntervention ficheIntervention;
        ParametrageDocument param;


        public PdfIntervention(FicheIntervention FicheIntervention, Parametrages parametrage)
        {
            ficheIntervention = FicheIntervention;
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
                doc.NewPage();

                doc.Add(Header());
                doc.Add(HeaderAdresse());
                BodyPrtestation();
                doc.Add(Rapport());
                doc.Add(Signature());
                //doc.Add(PiedPage());
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

        private string formatString(string textToFormat)
        {
            return new Regex("<[^>]+>").Replace(textToFormat.Replace("&nbsp;", "\n").Replace("<br>", "\n").Replace("<div>", "\n"), string.Empty);
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
                leftRowseco.AddCell(new PdfPCell(CreateSimpleHtmlParagraph(param.entete_interventions == null ? "" : (param.entete_interventions))) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_LEFT, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_MIDDLE });

                //leftRowseco.AddCell(new PdfPCell(new Paragraph(param.entete == null ? "" : (formatString(param.entete_interventions)), fontH8)) { BorderWidth = 0, PaddingLeft = 15f, HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL });


                leftRow.AddCell(new PdfPCell(leftRowseco) { BorderWidth = 0 });

                leftRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                // Right table
                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;


                PdfPTable secondRight = new PdfPTable(1);

                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                rightRow.AddCell(new PdfPCell(new Paragraph("FICHE INTERVENTION", fontH10Bold)) { PaddingLeft = 8f, BorderWidth = 0 });
                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });
                //info client
                //Phrase phraseCodeClient = new Phrase();

                //phraseCodeClient.Add(new Chunk("Code Client  : ", fontH12Bold));
                //phraseCodeClient.Add(new Chunk(ficheIntervention.Chantier.Client.Codeclient, fontH10));
                //rightRow.AddCell(new PdfPCell(phraseCodeClient) { BorderWidth = 0, PaddingLeft = 10f, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });

                //Phrase phraseNomClient = new Phrase();

                //phraseNomClient.Add(new Chunk("NOM Client  : ", fontH12Bold));
                //phraseNomClient.Add(new Chunk(ficheIntervention.Chantier.Client.Nom, fontH10));
                //rightRow.AddCell(new PdfPCell(phraseNomClient) { BorderWidth = 0, PaddingLeft = 10f, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
                var codeClient = ficheIntervention.Chantier.Client.Codeclient;
                PdfPTable infoClient = new PdfPTable(6);
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Fiche Intervention N° ", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Date", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk("Code Client", fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(ficheIntervention.Reference, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(ficheIntervention.DateDebut.ToString("dd/MM/yyyy"), fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                infoClient.AddCell(new PdfPCell(new Paragraph(new Chunk(codeClient, fontH15))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                rightRow.AddCell(new PdfPCell(infoClient) { BorderWidth = 0, PaddingTop = 0 });

                //AdresseModel Adresse = null;
                //if (ficheIntervention.Chantier.Client.Adresses != null)
                //{
                //    List<AdresseModel> adresses = JsonConvert.DeserializeObject<List<AdresseModel>>(ficheIntervention.Chantier.Client.Adresses);
                //    Adresse = adresses.FirstOrDefault();
                //}

                //Phrase phraseAdresse = new Phrase();
                //Phrase phraseCPVille = new Phrase();

                //if (Adresse.@default == true)
                //{
                //    phraseAdresse.Add(new Chunk("Adresse : ", fontH12Bold));
                //    phraseAdresse.Add(new Chunk((Adresse == null) ? " " : Adresse.adresse + " " + Adresse.complementAdresse + " " + Adresse.codePostal + " " + Adresse.ville, fontH10));
                //   rightRow.AddCell(new PdfPCell(phraseAdresse) { BorderWidth = 0, PaddingLeft = 10f, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
                    
                //}
                rightRow.AddCell(new PdfPCell() { FixedHeight = 5f, BorderWidth = 0 });


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
                if (ficheIntervention.AdresseIntervention != null)
                {
                    AdresseModel adressesi = JsonConvert.DeserializeObject<AdresseModel>(ficheIntervention.AdresseIntervention);
                    AdresseIntervention = adressesi;
                }
                leftRow.AddCell(new Paragraph("ADRESSE INTERVENTION :", fontH10B));
                leftRow.AddCell(new PdfPCell(new Paragraph(new Chunk((AdresseIntervention == null) ? " " : AdresseIntervention.complementAdresse, fontH10))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                leftRow.AddCell(new PdfPCell(new Paragraph(new Chunk((AdresseIntervention == null) ? " " : AdresseIntervention.codePostal + " " + AdresseIntervention.ville, fontH10))) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                table_header.AddCell(new PdfPCell(leftRow) { BorderWidth = 0, PaddingTop = 10 });

                PdfPTable rightRow = new PdfPTable(1);
                rightRow.WidthPercentage = 80;
                rightRow.DefaultCell.Border = Rectangle.NO_BORDER;
                //Date 

                Phrase phraseDateDebut = new Phrase();

                var datedebut = (ficheIntervention.DateDebut == null) ? "" : ficheIntervention.DateDebut.ToString("dd/MM/yyyy");

                phraseDateDebut.Add(new Chunk("Date début : ", fontH12Bold));
                phraseDateDebut.Add(new Chunk(datedebut, fontH15));
                rightRow.AddCell(new PdfPCell(phraseDateDebut) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });

                Phrase phraseDateFin = new Phrase();

                var dateFin = (ficheIntervention.DateFin == null) ? "" : ficheIntervention.DateFin.ToString("dd/MM/yyyy");

                phraseDateFin.Add(new Chunk("Date Fin : ", fontH12Bold));
                phraseDateFin.Add(new Chunk(dateFin,  fontH15));
                rightRow.AddCell(new PdfPCell(phraseDateFin) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });


                Phrase phraseNbrDeplacement = new Phrase();
                var nbrdep = (ficheIntervention.NombreDeplacement == null) ? "" : ficheIntervention.NombreDeplacement.ToString();
                phraseNbrDeplacement.Add(new Chunk("Nombre Déplacement : ", fontH12Bold));
                phraseNbrDeplacement.Add(new Chunk(nbrdep, fontH15));
                rightRow.AddCell(new PdfPCell(phraseNbrDeplacement) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });

                Phrase phraseNbrPanier = new Phrase();
                var nbrpanier = (ficheIntervention.NombrePanier == null) ? "" : ficheIntervention.NombrePanier.ToString();
                phraseNbrPanier.Add(new Chunk("Nombre Panier : ", fontH12Bold ));
                phraseNbrPanier.Add(new Chunk(nbrpanier, fontH15));
                rightRow.AddCell(new PdfPCell(phraseNbrPanier) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });

                

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

                if (ficheIntervention.Objet != null && ficheIntervention.Objet != "")
                {
                    articlestop.AddCell(new PdfPCell(new Paragraph(new Chunk(ficheIntervention.Objet, fontH10B))) { BorderWidth = 0.75f, BorderColor = borderBotomColor, Padding = 5f, BackgroundColor = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });
                    prestations.AddCell(new PdfPCell(articlestop) { BorderWidth = 0, PaddingBottom = 10 });
                    doc.Add(prestations);
                }



                PdfPTable articlestab = new PdfPTable(11)
                { SpacingBefore = 5f, WidthPercentage = 100 };
                articlestab.DefaultCell.Border = Rectangle.NO_BORDER;

                // Header table
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Article", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Désignation", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Catégorie", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });

                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("Quantité", fontH10B))) { FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });



                PrestationsModule DataPres = null;
                List<Data> LotProduit = null;
                if (ficheIntervention.Prestations != null)
                {


                    dynamic Prest = JsonConvert.DeserializeObject<Prestation>(ficheIntervention.Prestations);
                    List<PrestationsModule> Prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(Prest.prestation);

                    var length = Prestations.Count;

                    for (int i = 0; i < length; i++)
                    {
                        DataPres = Prestations[i];
                        Data DataPrestations = DataPres.data;
                        if (DataPres.type == (int)typePrestation.Produit || DataPrestations.lotProduits == null)
                        {
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.categorie, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });

                            articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0}", Prestations[i].data.qte), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });

                        }
                        else
                        {
                            if (DataPres.type == (int)typePrestation.Lot || DataPrestations.lotProduits != null)
                            {

                                List<DataLotProduit> ProduitLot = DataPrestations.lotProduits;
                                var taille = ProduitLot.Count;
                                articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(Prestations[i].data.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BackgroundColor = borderBotomColor, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 11 });

                                try
                                {
                                    for (int j = 0; j < taille; j++)
                                    {

                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk("     " + ProduitLot[j].idProduitNavigation.reference, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(ProduitLot[j].idProduitNavigation.nom, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });
                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(ProduitLot[j].idProduitNavigation.categorie, fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 });

                                        long qte = ProduitLot[j].idProduitNavigation.qte;
                                        articlestab.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0}", ProduitLot[j].idProduitNavigation.qte), fontH15))) { BorderWidthBottom = 0.3f, BorderWidthLeft = 0, BorderWidth = 0, Padding = 3f, BorderColorBottom = borderBotomColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2 });


                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, ex.Message);
                                    return null;
                                }
                

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


        public PdfPTable Rapport()
        {
            try
            {
                PdfPTable rapport = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 };
               
                rapport.DefaultCell.Border = Rectangle.NO_BORDER;
                rapport.WidthPercentage = 100;
                rapport.AddCell(new PdfPCell() { FixedHeight = 10f, BorderWidth = 0 });

                rapport.AddCell(new PdfPCell(new Paragraph("Rapport", fontH10)) { PaddingLeft = 50f, FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, });
                rapport.AddCell(new PdfPCell(new Paragraph(ficheIntervention.Rapport == null ? "" : (formatString(ficheIntervention.Rapport)), fontH10)) { BorderWidth = 0, PaddingLeft = 15f, PaddingRight = 80f, VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL });
          
                return rapport;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }


        public PdfPTable Signature()
        {
            try
            {
           
                PdfPTable signaturevisa = new PdfPTable(1) { SpacingBefore = 5f, WidthPercentage = 100 };
                signaturevisa.SpacingAfter = 15f;
                signaturevisa.DefaultCell.Border = Rectangle.NO_BORDER;
                PdfPTable visa = new PdfPTable(1) {  WidthPercentage = 100 };
                visa.DefaultCell.Border = Rectangle.NO_BORDER;
                PdfPTable signature = new PdfPTable(2) { WidthPercentage = 100 };

                signature.DefaultCell.Border = Rectangle.NO_BORDER;
                // signature.SetWidths(new float[] { 350f, 70f, 550f });
                visa.AddCell(new PdfPCell(new Paragraph(new Chunk("Visa", fontH10))) { PaddingLeft = 50f, FixedHeight = 20f, BorderWidth = 0, BackgroundColor = tableHeaderColor, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                signaturevisa.AddCell(new PdfPCell(visa) { BorderWidth = 0, PaddingTop = 10 });
                PdfPTable signature_technicien = new PdfPTable(1) ;
                signature_technicien.DefaultCell.Border = Rectangle.NO_BORDER;
                signature_technicien.WidthPercentage = 100;
                PdfPTable signature_client = new PdfPTable(1);

               // SignatureModel signetureClient= JsonConvert.DeserializeObject<SignatureModel>(ficheIntervention.SignatureClient);

                //Signature tECHNICIEN

                signature_technicien.AddCell(new PdfPCell(new Paragraph(new Chunk("Technicien", fontH12Bold))) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE});
                Phrase PhrasesignetureTechnom = new Phrase();
                Phrase PhrasesignetureTechdate = new Phrase();
                Phrase PhrasesignetureTechsignature = new Phrase();
                SignatureModel signatureTech = null;

             
                if (ficheIntervention.SignatureTechnicien != null)
                {
                    SignatureModel signetureTechnicien = JsonConvert.DeserializeObject<SignatureModel>(ficheIntervention.SignatureTechnicien);
                    signatureTech = signetureTechnicien;

                }

               
                PhrasesignetureTechnom.Add(new Chunk("Nom : ", fontH12Bold));
                PhrasesignetureTechnom.Add(new Chunk((signatureTech == null) ? " " : signatureTech.nom, fontH10));
                PhrasesignetureTechdate.Add(new Chunk("Date : ", fontH12Bold));
                if (signatureTech != null)
                {
                    var SignaturedateTech = (signatureTech.date == null) ? "" : signatureTech.date.ToString("dd/MM/yyyy");

                    PhrasesignetureTechdate.Add(new Chunk(SignaturedateTech, fontH10));
                }
                    

                PhrasesignetureTechsignature.Add(new Chunk("Signature : ", fontH12Bold));
               // PhrasesignetureTechsignature.Add(new Chunk((signatureTech == null) ? " " : signatureTech.signature, fontH10));
                PdfPTable signatureTable = new PdfPTable(1);
             if (signatureTech != null)
                {
                    if (!string.IsNullOrEmpty(signatureTech.signature))
                    {
                        ConvertBase64ToImage convert = new ConvertBase64ToImage();
                        string content = convert.Replace(signatureTech.signature);
                        if (convert.IsBase64(content))
                        {
                            byte[] imageBytes = Convert.FromBase64String(content);
                            System.Drawing.Image result = convert.ByteArrayToImage(imageBytes);
                            Image Signatu = Image.GetInstance(result, null, false);
                            Signatu.ScaleAbsolute(160f, 160f);

                            signatureTable.AddCell(new PdfPCell(Signatu) { BorderWidth = Rectangle.NO_BORDER });
                        }
                        else { signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { BorderWidth = Rectangle.NO_BORDER }); }
                         
                    }
                }
                else { signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { BorderWidth = Rectangle.NO_BORDER }); }
                   
                signature_technicien.AddCell(new PdfPCell(PhrasesignetureTechnom) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });
                signature_technicien.AddCell(new PdfPCell(PhrasesignetureTechdate) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });
                signature_technicien.AddCell(new PdfPCell(PhrasesignetureTechsignature) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });

                signature_technicien.AddCell(new PdfPCell(signatureTable) { BorderWidth = 0.75f, BorderColor = borderTopColor, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });

                signature.AddCell(new PdfPCell(signature_technicien) { BorderWidth = 0 });
           
                //Signature CLIENT


                Phrase phrasesignatureClientnom = new Phrase();
                Phrase phrasesignatureClientdate = new Phrase();
                Phrase phrasesignatureClientsignature = new Phrase();

                signature_client.AddCell(new PdfPCell(new Paragraph(new Chunk("Client", fontH12Bold))) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE});
              
                SignatureModel SignatureClient = null;

             
                if (ficheIntervention.SignatureClient != null)
                {
                    SignatureModel signatureClient = JsonConvert.DeserializeObject<SignatureModel>(ficheIntervention.SignatureClient);
                    SignatureClient = signatureClient;

                }

               
                phrasesignatureClientnom.Add(new Chunk("Nom : ", fontH12Bold));
                phrasesignatureClientnom.Add(new Chunk((SignatureClient == null) ? " " : SignatureClient.nom, fontH10));
                phrasesignatureClientdate.Add(new Chunk("Date : ", fontH12Bold));
               
                 if(SignatureClient != null)
                {
                     var Signaturedateclient = (SignatureClient.date == null) ? "" : SignatureClient.date.ToString("dd/MM/yyyy");

                phrasesignatureClientdate.Add(new Chunk( Signaturedateclient, fontH10));

                }
               

                phrasesignatureClientsignature.Add(new Chunk("Signature : ", fontH12Bold));
                PdfPTable signatureClientTable = new PdfPTable(1);

                if (SignatureClient != null)
                {
                    if (!string.IsNullOrEmpty(SignatureClient.signature))
                    {
                        ConvertBase64ToImage convert = new ConvertBase64ToImage();
                        string content = convert.Replace(SignatureClient.signature);
                        if (convert.IsBase64(content))
                        {
                            byte[] imageBytes = Convert.FromBase64String(content);
                            System.Drawing.Image result = convert.ByteArrayToImage(imageBytes);
                            Image Signatu = Image.GetInstance(result, null, false);
                            Signatu.ScaleAbsolute(160f, 160f);

                            signatureClientTable.AddCell(new PdfPCell(Signatu) { BorderWidth = Rectangle.NO_BORDER });
                        }
                        else
                            signatureClientTable.AddCell(new PdfPCell(new Phrase(" ")) { BorderWidth = Rectangle.NO_BORDER });
                    }
                }
                else{
                    signatureClientTable.AddCell(new PdfPCell(new Phrase(" ")) { BorderWidth = Rectangle.NO_BORDER });
                }

                signature_client.AddCell(new PdfPCell(phrasesignatureClientnom) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });
                signature_client.AddCell(new PdfPCell(phrasesignatureClientdate) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });
                signature_client.AddCell(new PdfPCell(phrasesignatureClientsignature) { BorderWidth = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });

                signature_client.AddCell(new PdfPCell(signatureClientTable) { BorderWidth = 0.75f, Padding = 5f, BorderColor = borderTopColor, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_LEFT });

                signature.AddCell(new PdfPCell(signature_client) { BorderWidth = 0 });
                 
                signature.AddCell(new PdfPCell() { BorderWidth = 0 });
                signaturevisa.AddCell(new PdfPCell(signature) { BorderWidth = 0, PaddingTop = 10 });

                return signaturevisa;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
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
