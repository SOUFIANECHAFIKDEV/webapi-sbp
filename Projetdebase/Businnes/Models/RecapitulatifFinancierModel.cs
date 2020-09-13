using ProjetBase.Businnes.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class RecapitulatifFinancierModel
    {
        public List<PrevisionnelElementsModel> previsionnel;
        public  List<FacturationTresorieModel> facturation_tresorie;
        public double tresorerieChantier;
    }

    public class PrevisionnelElementsModel
    {
        public TypePrevisionnel typeElements;
        public double? sum;
        public List<ElementsPrevisionnelModel> elements;
        public double? pourcentage;
        public double? defference;

    }


    public class ElementsPrevisionnelModel
    {
        public int type;
        public double? sum;
        public double? pourcentage;
        public double? defference;
        public double? NbrHeure;
        public string souselements;

    }


    public class FacturationTresorieModel
    {
        public typeFactureTresorie typeElements;
        public double? sum;
        public List<ElementsFacturationTresorieModel> elements;
        public double? pourcentage;
        public double? defference;

    }
    public class ElementsFacturationTresorieModel
    {
        public int type;
        public double? sum;
        public double? pourcentage;
        public double? defference;
        public double? NbrHeure;
        public List<SousElementsFacturationTresorieModel> souselements;
    }
    public class SousElementsFacturationTresorieModel
    {
        public int typeSousElement;
        public double? sum;
        public double? pourcentage;
        public double? NbrPanierOrDeplacement;
    }
}
