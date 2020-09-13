

namespace ProjetBase.Businnes.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using ProjetBase.Businnes.Contexts;
    using ProjetBase.Businnes.Models.Paging;
    using ProjetBase.Businnes.Models.Sorting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// La classe EntityFrameworkRepository.
    /// </summary>
    /// <typeparam name="TEntity">The first generic type parameter.</typeparam>
    /// <typeparam name="TKey">The second generic type parameter.</typeparam>
    public class EntityFrameworkRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Context de connexion.
        /// </summary>
        private readonly ProjetBaseContext contextDB;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EntityFrameworkRepository{TEntity,TKey}" />
        /// </summary>
        /// <param name="dbcontext">Context de la BD</param>
        public EntityFrameworkRepository(ProjetBaseContext dbcontext)
        {
            if (dbcontext == null)
            {
                throw new ArgumentNullException("dbContext");
            }

            this.contextDB = dbcontext;
        }

        /// <summary>
        /// Obtient una context de DB
        /// </summary>
        protected ProjetBaseContext DbContext
        {
            /// <summary>
            /// définit le context de connexion
            /// </smmary>
            get { return this.contextDB; }
        }

        /// <summary>
        /// Récuperer tous les enregistrements
        /// </summary>
        /// <returns>Renvoie tous les enregistrements</returns>
        public IEnumerable<TEntity> GetAll()
        {
            return this.contextDB.Set<TEntity>().ToList();
        }

        /// <summary>
        /// Récuperer un enregistrement
        /// </summary>
        /// <param name="id">l'id de l'enregistrement</param>
        /// <returns>Renvoie tous les enregistrements</returns>
        public TEntity GetById(TKey id)
        {
            return this.contextDB.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Récuperer une liste d'enregistrements
        /// </summary>
        /// <param name="filter">Le filtre</param>
        /// <param name="orderBy">L'ordre de tri</param>
        /// <param name="include">>whether to load the related entities or not</param>
        /// <returns>Renvoie tous les enregistrements</returns>
        public IEnumerable<TEntity> Get(
                                        Expression<Func<TEntity, bool>> filter = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                        string include = "")
        {
            return this.GetQueryable(filter, orderBy, include).ToList();
        }

        /// <summary>
        /// Créer un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a sauvgardé</param>
        public virtual void Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DbContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Mise à jour d'un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a mettre à jour</param>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DbContext.Set<TEntity>().Attach(entity);
            this.DbContext.Entry(entity).State = EntityState.Modified;//System.Data.Entity.EntityState.Modified;


        }

        /// <summary>
        /// Suppression d'un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a supprimer</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DbContext.Set<TEntity>().Attach(entity);
            this.DbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Récuperer une liste d'enregistrements
        /// </summary>
        /// <param name="filter">Le filtre</param>
        /// <param name="orderBy">L'ordre de tri</param>
        /// <param name="include">>whether to load the related entities or not</param>
        /// <returns>Renvoie tous les enregistrements</returns>
        protected IQueryable<TEntity> GetQueryable(
                                                    Expression<Func<TEntity, bool>> filter = null,
                                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                   string include = "")
        {
            IQueryable<TEntity> query = this.contextDB.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = this.BuildQueryWithEntities(query, include);

            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            else
            {
                return query.AsQueryable();
            }
        }

        /// <summary>
        /// Suppression d'un enregistrement
        /// </summary>
        /// <param name="query">La requete</param>
        /// <param name="includeEntities">>whether to load the related entities or not</param>
        /// <returns>Un objet IQueryable</returns>
        internal IQueryable<TEntity> BuildQueryWithEntities(IQueryable<TEntity> query, string includeEntities)
        {
            if (string.IsNullOrEmpty(includeEntities))
            {
                return query;
            }

            foreach (string includeEntity in includeEntities.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeEntity.Trim());
            }

            return query;
        }

        public PagedList<TEntity> Filter(PagingParams pagingParams, Expression<Func<TEntity, bool>> filter = null,SortingParams sortingParams = null,string include = "")
        {
            IQueryable<TEntity> query = this.contextDB.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = this.BuildQueryWithEntities(query, include);

            if (sortingParams != null)
            {
                query = new SortedList<TEntity>(query, sortingParams).GetSortedList();
            }

            return new PagedList<TEntity>(query, pagingParams.PageNumber, pagingParams.PageSize);
        }

        public TEntity GetById(Expression<Func<TEntity, bool>> filter = null, string include = "")
        {
            IQueryable<TEntity> query = this.contextDB.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = this.BuildQueryWithEntities(query, include);

            return query.FirstOrDefault();
        }
    }
}
