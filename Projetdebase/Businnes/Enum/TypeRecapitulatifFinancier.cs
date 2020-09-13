using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Enum
{
    public enum TypeRecapitulatifFinancier
    {
        previsionnel = 1,
        facturation_tresorie = 2,
    }

   public enum TypePrevisionnel
    {
        total_devis = 1,
        depensee_Aprevoir = 2,
        marge_Previsionnel = 3,
    }

   

    public enum total_devis
    {
        vente_materiel = 1,
        vente_main_oveure = 2,
    }

    public enum depenseAprevoir
    {
        achat_materiel = 1,
        achat_main_oveure = 2,
        sous_traitance = 3,
    }
    public  enum MargePrevisionnel
    {
        retenueGarantier = 1,
        margeMateriel = 2,
        margeMainOeuvre = 3,
    }
    public enum typeFactureTresorie
    {
        caFacture = 1,
        depenseeEngagees = 2,
        margeReele = 3,
    }

    public enum caFacture
    {
        payes = 1,
        enAttentepaiement = 2,
    }
    public  enum depenseEngagees
    {
        achatsMateriels = 1,
        interventions = 2,
        sousTraitance = 3,
    }

    public enum depenseAchat
    {
        payes = 1,
        enAttentepaiement = 2,
        
    }

    public enum depenseSousTraitent
    {
        payes = 1,
        enAttentepaiement = 2,

    }
    public enum InterventionsousElement
    {
        panier = 1,
        deplacement = 2,

    }
    public enum MargeReelle
    {
        retenueGarantier = 1,
        margeMateriel = 2,
        margeMainOeuvre = 3,
    }




}
