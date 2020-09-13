using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetBase.Businnes.Repositories
{
    using ProjetBase.Businnes.Models.Paging;
    using ProjetBase.Businnes.Models.Sorting;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// L'interface IRepository.
    /// </summary>
    /// <typeparam name="TEntity">The first generic type parameter.</typeparam>
    /// <typeparam name="TKey">The second generic type parameter.</typeparam>
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Récuperer tous les enregistrements
        /// </summary>
        /// <returns>Renvoie tous les enregistrements</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Récuperer un enregistrement
        /// </summary>
        /// <param name="id">L'id de l'enregistrement</param>
        /// <returns>Renvoie un enregistrement</returns>
        TEntity GetById(TKey id);

        /// <summary>
        /// Créer un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a sauvgardé</param>
        void Create(TEntity entity);

        /// <summary>
        /// Mise à jour d'un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a mettre à jour</param>
        void Update(TEntity entity);

        /// <summary>
        /// Suppression d'un enregistrement
        /// </summary>
        /// <param name="entity">L'enregistrement a supprimer</param>
        void Delete(TEntity entity);


        PagedList<TEntity> Filter(PagingParams pagingParams, Expression<Func<TEntity, bool>> filter = null,
                                                   SortingParams sortingParams = null,
                                                   string include = "");

        TEntity GetById(Expression<Func<TEntity, bool>> filter = null, string include = "");
    }
}
