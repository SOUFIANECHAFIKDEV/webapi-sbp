using ProjetBase.Businnes.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class ChangeStatutBodyRequest
    {
        public int idChantier { get; set; }
        public StatutChantier statutChantier { get; set; }
    }
    public class changementTauxAvencement
    {
        public int idChantier { get; set; }
        public int tauxAvencement { get; set; }
    }

    public class ChangeStatutBodyRequestDevis
    {
        public int iddevis{ get; set; }
        public StatutDevis statutDevis { get; set; }
    }


    public class ChangeStatutBodyRequestFicheIntervention
    {
        public int idficheIntervention { get; set; }
        public StatutFicheIntervention statutficheIntervention { get; set; }
    }

    public class ChantierChangeStatusResponse
    {
        public Enum.ChantierChangeStatusResponse result;
        public Entities.Chantier chantier;
    }

    public class ChangeStatusRetenueGarantieResponse
    {
        public int idFacture{ get; set; }
        public int StatusRetenueGarantie { get; set; }
    }

    public class StatusRetenueGarantie
    {
        public Enum.ChantierChangeStatusResponse result;
        public Entities.Chantier chantier;
    }

    public class ChangeStatutBodyRequestContratEntretien
    {
        public int idContratEntretien { get; set; }
        public StatutContratEntretien statutContratEntretien { get; set; }
    }

    public class ChangeStatutBodyRequestVisiteMaintenance
    {
        public int idVisiteMaintenance { get; set; }
        public StatutVisiteMaintenance statutVisiteMaintenance { get; set; }
    }
}
