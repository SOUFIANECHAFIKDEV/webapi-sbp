using iTextSharp.text;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.PDF_FactureParPeriod
{
    interface IPDFFactureParPeriod
    {
        iTextSharp.text.Document CreateDocument();
        void CloseDocument(iTextSharp.text.Document doc);
        void OpenDocument(iTextSharp.text.Document doc);
        void Header(iTextSharp.text.Document document, Client chantier, ExportFactureEtAvoirParPeriodModel paramsBody, Parametrages param, int typeDoc);
        void HeaderFactureRelance(iTextSharp.text.Document document, Client chantier, exportReleveRelanceFactureModel paramsBody, Parametrages param, int typeDoc);
        void BodyArticles(iTextSharp.text.Document document, List<Facture> Facture, List<Avoir> avoirs);
        void BodyArticlesRelence(iTextSharp.text.Document document, List<Facture> Facture, List<Avoir> avoirs);

        
        byte[] GeneratePDFWriter(iTextSharp.text.Document doc, MemoryStream memStream);
    }
}
