using iTextSharp.text;
using ProjetBase.Businnes.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.Pdf_Document
{
    interface IPdf_Document
    {
        //byte[] GenerateDocument();

        iTextSharp.text.Document CreateDocument();
        void CloseDocument(iTextSharp.text.Document doc);
        void OpenDocument(iTextSharp.text.Document doc);
        //void DefinerArticles(Facture FA);
        byte[] GeneratePDFWriter(iTextSharp.text.Document doc, MemoryStream memStream);
        void Header(iTextSharp.text.Document document, dynamic _doc, Entities.Parametrages ParametrageDocument);
        //List<IGrouping<int, FournisseurGrouped>> ArticleFournisseur();
        void BodyArticles(iTextSharp.text.Document document, dynamic _doc);
        //void HeaderAdresee(iTextSharp.text.Document document, dynamic _doc);
        void calculTva(iTextSharp.text.Document document, dynamic _doc);
        void conditionREG(iTextSharp.text.Document document, dynamic _doc, Parametrages ParametrageDocument);
        void PiedPage(iTextSharp.text.Document document, dynamic _doc, Parametrages ParametrageDocument);
      
         
    }
}
