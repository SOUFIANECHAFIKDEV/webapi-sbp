using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class Parametrage
    {
        public string Racine { get; set; }
        public int FormatDate { get; set; }
        public int Compteur { get; set; }
        public int? LongeurCompteur { get; set; }
        public int Type { get; set; }
    }

    public class ParametrageDocument
    {
        public string entete { get; set; }
        public string note { get; set; }
        public string conditions { get; set; }
        public string entete_interventions { get; set; }
        public string entete_facture { get; set; }
        public string note_facture { get; set; }
        public string conditions_facture { get; set; }
        public int validite_avoir { get; set; }
        public string entete_avoir { get; set; }
        public string note_avoir { get; set; }
        public string conditions_avoir { get; set; }
    }
    public class ParametrageAgendaGoogle
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string applicationName { get; set; }
        public string calendarId { get; set; }

    }
    public class coutsHoraireModel
    {
        public double prixVente { get; set; }
        public double prixAchat { get; set; }
        public double coutPanier { get; set; }
        public double coutDeplacement { get; set; }
    }

    public class ParametrageHoraireTravail
    {
        public string heureDebut { get; set; }
        public string heureFin { get; set; }
    }
    //public class ParametrageIntervention
    //{
    //    public string entete { get; set; }

    //}

}
