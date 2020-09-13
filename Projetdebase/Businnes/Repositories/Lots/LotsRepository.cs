using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Lots
{
    public class LotsRepository : EntityFrameworkRepository<Entities.Lots, int>, ILotsRepository
    {
        public ProjetBaseContext SbpContext;
        public LotsRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
        }

        public PagedList<Entities.Lots> Filter(PagingParams pagingParams, Expression<Func<Entities.Lots, bool>> filter = null, SortingParams sortingParams = null)
        {
            IQueryable<Entities.Lots> query = DbContext.Set<Entities.Lots>();
            query = query.Where(filter).Include(f => f.LotProduits).ThenInclude(u => u.IdProduitNavigation).ThenInclude(x=>x.PrixParFournisseur)/*.Include(x => x.Societe).Include(x => x.Pays)*/;
            //if (sortingParams != null)
            //{
            //    query = new SortedList<Entities.Lots>(query, sortingParams).GetSortedList();
            //}

            return new PagedList<Entities.Lots>(query, pagingParams.PageNumber, pagingParams.PageSize);
        }

        public bool supprimerLots(int id)
        {
            var Lot = DbContext.Lots.SingleOrDefault(C => C.Id == id);
            DbContext.Remove(Lot);
            DbContext.SaveChanges();
            return true;
        }

        //verifier que le nom du lot est unique
        public bool CheckUniqueReference(string nom)
        {
            var NbrReference = DbContext.Lots.Where(x => x.Nom == nom).Count();
            return NbrReference > 0;
        }

        public void CreateLots(Entities.Lots entity)
        {
            try
            {
                List<Entities.LotProduits> lotProduits = new List<Entities.LotProduits>();

                foreach (var produit in entity.LotProduits)
                {

                    Entities.LotProduits produits = new Entities.LotProduits
                    {
                        Id = -1,
                        IdLot = produit.IdLot,
                        IdLotNavigation = produit.IdLotNavigation,
                        IdProduitNavigation = SbpContext.Produit.Where(x => x.Id == produit.IdProduit).Include(x => x.PrixParFournisseur).FirstOrDefault() /*produit.IdProduitNavigation*/,
                        IdProduit = 1000,
                        Qte = produit.Qte
                    };

                    lotProduits.Add(produits);


                }

                var body = new Entities.Lots
                {
                    Description = entity.Description,
                    Nom = entity.Nom,
                    LotProduits = lotProduits
                };


                SbpContext.Add(body);
                SbpContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
