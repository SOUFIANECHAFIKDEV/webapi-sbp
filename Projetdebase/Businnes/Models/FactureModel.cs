using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;

namespace ProjetBase.Businnes.Repositories
{
    public class FactureAllModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public List<int> Statut { get; set; }
        //public int? Statut { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int? IdClient { get; set; }
        public int? IdChantier { get; set; }
    }

    public class ExportFactureEtAvoirParPeriodModel
    {
        public int? IdClient { get; set; }
        public int? IdChantier { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool ExporterFacturesChantier;
        public bool ExporterFacturesClient;
    }
    public class exportReleveRelanceFactureModel
    {
        public int? IdClient { get; set; }
        public int? IdChantier { get; set; }
        public bool ExporterFacturesClient;
        public bool ExporterFacturesChantier;
    }

    public class FacturesAndAvoirs
    {
        public Entities.Client Client { get; set; }
        public Entities.Chantier Chantier { get; set; }
        public List<FacturesPdf> factures { get; set; }
        public List<FacturesPdf> avoirs { get; set; }

    }

    public class RelanceFactures
    {
        public Entities.Chantier Chantier { get; set; }

        public Entities.Client Client { get; set; }
        public List<FacturesPdf> factures { get; set; }
    }

    public class FacturesPdf
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public DateTime? DateCreation { get; set; }
        public double? Montant { get; set; }
        public double? Reste { get; set; }
        public string type { get; set; }
    }

    public class AvoirsPdf
    {
        public string Reference { get; set; }
        public DateTime DateCreation { get; set; }
        public double Montant { get; set; }
        public string type { get; set; }
    }

    public class FactureGenerateReferenceModel
    {
        public DateTime DateCreation { get; set; }
    }

    public class FactureReference
    {
        public string Reference { get; set; }
        public int? Compteur { get; set; }
        public int Statut { get; set; }
        public bool IsOld { get; set; }
    }

    public class FacturePostModel
    {
        public Entities.Facture Facture { get; set; }

        public List<int> FicheInterventionIds { get; set; }

    }

    public class FactureModel
    {
        public int validite { get; set; }
        public string sujetmail { get; set; }
        public string contenumail { get; set; }
        public string sujetrelance { get; set; }
        public string contenurelance { get; set; }
        public string headerfacture { get; set; }
        public string piedfacture { get; set; }
        public string conditionReglementFacture { get; set; }
    }

    public class FactureSituationModal
    {
        public Entities.Facture facture { get; set; }
        public int idDevis { get; set; }
        public FactureSituationDevis situations { get; set; }
    }

    public class FactureSituationDevis
    {
        public int? idFacture { get; set; }
        public double pourcentage { get; set; }
        public double resteAPayer { get; set; }
        public double resteAPayerHT { get; set; }
        public double situationCumulleeTTC { get; set; }
        public double situationCumulleeHT { get; set; }
        public double acomptesCumulleeHT { get; set; }
        public double acomptesCumulleeTTC { get; set; }
    }

    public class FactureAcomptesModal
    {
        public Entities.Facture facture { get; set; }
        public int idDevis { get; set; }
        public FactureAcompteDevis Acomptes { get; set; }
    }

    public class FactureAcompteDevis 
    {
        public int? idFacture { get; set; }
        public double pourcentage { get; set; }
        public double resteAPayer { get; set; }
        public double resteAPayerHT { get; set; }
        public double acomptesCumulleeHT { get; set; }
        public double acomptesCumulleeTTC { get; set; }
    }

}
