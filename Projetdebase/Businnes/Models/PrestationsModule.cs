using ProjetBase.Businnes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class Prestation
    {
        public dynamic prestation { get; set; }
        //public double totalHt { get; set; }
        //public double totalTtc { get; set; }
        //public string tva { get; set; }
        //public double remise { get; set; }
        public string typeRemise { get; set; }
        //public double prorata { get; set; }
        //public double puc { get; set; }
        //public double tvaGlobal { get; set; }
        //public bool retenueGarantie { get; set; }
        //public object totalTVA { get; set; }
    }
    public class PrestationsModule
    {

        public Data data { get; set; }
        public long qte { get; set; }
        public long type { get; set; }
        public long remise { get; set; }
    }

    public class ArticleModel
    {
        public int id { get; set; }
        public string reference { get; set; }
        public string nom { get; set; }
        public double tva { get; set; }
        public string unite { get; set; }
        public double prix { get; set; }
        public string description { get; set; }
        public double qte { get; set; }
        public double remise { get; set; }
        public string typeRemise { get; set; }
        public double totalHT { get; set; }
        public double totalTTC { get; set; }
        public string categorie { get; set; }

      //  public List<PrixParFournisseur> prixParFournisseur { get; set; }
    }

    public partial class Data
    {
       
        public long id { get; set; }
        public string reference { get; set; }
        public string nom { get; set; }
        public string description { get; set; }
        public string designation { get; set; }
        public double nomber_heure { get; set; }
        public double cout_materiel { get; set; }
        public double cout_vente { get; set; }
        public double prixHt { get; set; }
        public long tva { get; set; }
        public object fichesTechniques { get; set; }
        public string historique { get; set; }
        public string unite { get; set; }
       //public Categorie? Categorie { get; set; }
        public string labels { get; set; }
        public long lot { get; set; }
        //public int id_fournisseur { get; set; }
        //public Fournisseur fournisseur { get; set; 
        public  List<ProduitFournisseur> prixParFournisseur { get; set; }
        public double prix_fournisseur { get; set; }
        public double cout_horaire { get; set; }
        // public DataLotProduit[] LotProduits { get; set; }
        public List<DataLotProduit> lotProduits { get; set; }
        public long qte { get; set; }
        public long? remise { get; set; }
        public string categorie { get; set; }
    }


    public partial class DataLotProduit
    {
      
        public long id { get; set; }
        public long qte { get; set; }
        public long idLot { get; set; }
        public long idProduit { get; set; }
        public Data  idProduitNavigation { get; set; }
        public long remise { get; set; }
    }

    public partial class PurpleIdProduitNavigation
    {
        public long id { get; set; }
        public string reference { get; set; }
        public string nom { get; set; }
        public string description { get; set; }
        public string designation { get; set; }
        public long nomber_heure { get; set; }
        public long cout_materiel { get; set; }
        public long cout_vente { get; set; }
        public double prixHt { get; set; }
        public long tva { get; set; }
        public string fichesTechniques { get; set; }
        public string historique { get; set; }
        public object unite { get; set; }
        public string categorie { get; set; }
        public object labels { get; set; }
        public long lot { get; set; }
        public int id_fournisseur { get; set; }
        public object fournisseur { get; set; }
        public long prix_fournisseur { get; set; }
        public long cout_horaire { get; set; }
        public List<IdProduitNavigationLotProduit> lotProduits { get; set; }
        public long qte { get; set; }
        public long remise { get; set; }
     
    }

    public partial class FluffyIdProduitNavigation
    {
      
        public long Id { get; set; }
        public string Reference { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Designation { get; set; }
        public long NomberHeure { get; set; }
        public long CoutMateriel { get; set; }
        public long CoutVente { get; set; }
        public long PrixHt { get; set; }
        public long Tva { get; set; }
        public string FichesTechniques { get; set; }
        public string Historique { get; set; }
       
        //public Unite Unite { get; set; }
        //public Categorie Categorie { get; set; }
        // public Labels Labels { get; set; }
        public long Lot { get; set; }
        public long IdFournisseur { get; set; }
        public object Fournisseur { get; set; }
        public long PrixFournisseur { get; set; }
        public long CoutHoraire { get; set; }
        public string categorie { get; set; }
        public List<IdProduitNavigationLotProduit> LotProduits { get; set; }
    }

    public partial class IdLotNavigation
    {
       public long Id { get; set; }
       public string Nom { get; set; }
       public string Description { get; set; }
       public List<IdProduitNavigationLotProduit> LotProduits { get; set; }
    }

    public partial class IdProduitNavigationLotProduit
    { 
        public long Id { get; set; }
        public long Qte { get; set; }
        public long IdLot { get; set; }
        public long IdProduit { get; set; }
        public IdLotNavigation IdLotNavigation { get; set; }
        public Data IdProduitNavigation { get; set; }
    }
      public class TvaModel
    {
        public double tva { get; set; }
        public double totalHT { get; set; }
        public double totalTVA { get; set; }
        public double totalTTC { get; set; }
    }

}
