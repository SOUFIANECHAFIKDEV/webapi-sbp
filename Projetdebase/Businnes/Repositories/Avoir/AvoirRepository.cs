using Newtonsoft.Json;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Parametrage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;

namespace ProjetBase.Businnes.Repositories.Avoir
{
    public class AvoirRepository : EntityFrameworkRepository<Entities.Avoir, int>, IAvoirRepository
    {

        private readonly IParametrageRepository parametrageRepository;

        public AvoirRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            parametrageRepository = new ParametrageRepository(dbContext);

        }

        // Get avoir
        public  Entities.Avoir GetAvoir(int id)
        {
            try
            {
                var avoir = DbContext.Avoirs.Where(x => x.Id == id)
                          .Include(x => x.Chantier).ThenInclude(x => x.Client)
                          .Include(x => x.Facture)
                           .Include(x => x.Paiements).ThenInclude(x => x.ModeReglement)
                          .FirstOrDefault();
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!! mauvaise implementaion !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                avoir.Client = DbContext.Clients.SingleOrDefault(X => X.Id == avoir.IdClient);
                //  if (avoir.IdChantier == null)
                //{

                //}


                return avoir;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }



        public async Task<bool> SaveMemos(int id, string memos)
        {
            try
            {
                var avoir = GetById(id);
                avoir.Memos = memos;
                Update(avoir);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

       
        // Get avoir chantier 
        public async Task<List<Entities.Avoir>> GetAvoirsClient(int IdClient, List<int> status, DateTime? DateDebut, DateTime? DateFin)
        {
            List<Entities.Avoir> AvoirsList = DbContext.Avoirs
                                                    .Where(A =>
                                                            status.Contains(A.Status)
                                                            && A.IdClient == IdClient
                                                            && (DateDebut == null ? true : DateDebut <= A.DateCreation.Date)
                                                            && (DateFin == null ? true : DateFin >= A.DateCreation.Date)
                                                     )
                                                    .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                                 
                                                    .ToList();
            return AvoirsList;
           
        }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.Avoirs.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        //public async Task<List<Entities.Avoir>> GetAvoirsClient(int IdClient, List<int> status, DateTime? DateDebut, DateTime? DateFin)
        //{
            
        //    List<Entities.Avoir> AvoirsList = new List<Entities.Avoir>();
        //    List<Entities.Chantier> Chantiers = DbContext.Chantier.Where(c => c.IdClient == IdClient).ToList();
        //    if (Chantiers.Count > 0)
        //    {
        //        foreach (var item in Chantiers)
        //        {
        //            List<Entities.Avoir> internalInvoices = DbContext.Avoirs.Where(a => a.IdChantier == item.Id).ToList();
        //            if (internalInvoices.Count > 0)
        //            {
        //                AvoirsList = AvoirsList.Concat(internalInvoices).ToList();
        //            }
        //        }
        //    }
        //    return AvoirsList.Where(a => a.DateCreation >= DateDebut && a.DateEcheance <= DateFin  ).ToList();
        //}

        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>

                {
                    var dd = h.Attribute;
                    if (h.Attribute != "Prestations" && h.Attribute != "Historique" && h.Attribute != "Chantier"   && h.Attribute != "Tva" && h.Attribute != "Historique" && h.Attribute != "UpdateAt"  && h.Attribute != "Memos" && h.Attribute != "InfoClient")
                    {
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                        
                            if (h.After != "")
                            {
                                int id = Convert.ToInt32(h.After);
                                h.After = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            else
                            {
                                h.After = null;
                            }
                         
                            if(h.Before != "")
                            {
                                int id = Convert.ToInt32(h.Before);
                                h.Before = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            else
                            {
                                h.Before = null;
                            }
                        }


                        if (h.Attribute == "IdClient")
                        {
                            h.Attribute = "Client";
                            if (h.After != "")
                            {
                                int id = Convert.ToInt32(h.After);
                                h.After = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            if (h.Before != "")
                            {
                                int id = Convert.ToInt32(h.Before);
                                h.Before = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                        }

                        if (h.Attribute == "Status")
                        {
                            h.After = getStatusAvoir(Int32.Parse(h.After));
                            h.Before = getStatusAvoir(Int32.Parse(h.Before));

                        }
                       
                        if (h.Attribute == "Total")
                        {
                            double a = Convert.ToDouble(h.After);
                            double b = Convert.ToDouble(h.Before);

                            h.After = String.Format("{0:0.00}", a);
                            h.Before = String.Format("{0:0.00}", b);
                        }
                        if (h.Attribute == "TypeRemise")
                        {
                            h.Attribute = "Type Remise";
                        }
                        if (h.Attribute == "Object")
                        {
                            h.Attribute = "Objet";
                        }
                        if (h.Attribute == "RetenueGarantie")
                        {
                            h.Attribute = "Retenue Garantie";
                            h.After = (h.After == "0" ? "Non" : "Oui");
                            h.Before = (h.Before == "0" ? "Non" : "Oui");
                        }

                        if (h.Attribute == "TotalHt")
                        {
                            double a = Convert.ToDouble(h.After);
                            double b = Convert.ToDouble(h.Before);

                            h.After = String.Format("{0:0.00}", a);
                            h.Before = String.Format("{0:0.00}", b);
                        }

                        NewhistoriqueChamps.Add(h);
                    }
                });

                return NewhistoriqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }


        // Get statut à partir de son numéro
        public string getStatusAvoir(int v)
        {
            try
            {
                var statut = "";
                switch (v)
                {
                    case (int)StatutAvoir.Brouillon:
                        statut = "Brouillon";
                        break;
                    case (int)StatutAvoir.Encours:
                        statut = "En cours";
                        break;
                    case (int)StatutAvoir.Utilise:
                        statut = "Utilisé";
                        break;
                    case (int)StatutAvoir.Expire:
                        statut = "Expiré";
                        break;
                    default:
                        statut = "";
                        break;
                }
                return statut;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

    }
}
