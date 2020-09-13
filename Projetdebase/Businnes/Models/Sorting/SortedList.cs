using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace ProjetBase.Businnes.Models.Sorting
{
    public class SortedList<T> where T : class
    {
        private IQueryable<T> Source { get; set; }
        public List<T> ListSource { get; private set; }
        private SortingParams SortingParams { get; set; }

        public SortedList(IQueryable<T> source, SortingParams sortingParams)
        {
            this.Source = source;
            this.SortingParams = sortingParams;
        }

        public SortedList(List<T> source, SortingParams sortingParams)
        {
            this.ListSource = source;
            this.SortingParams = sortingParams;
        }

        public IQueryable<T> GetSortedList()
        {
            try
            {
                string command = SortingParams.SortDirection == "desc" ? "OrderByDescending" : "OrderBy";
                var type = typeof(T);
                var property = type.GetProperty(SortingParams.OrderBy, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null && !string.IsNullOrEmpty(property.Name))
                {
                    var parameter = Expression.Parameter(type, property.Name);
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                    var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, this.Source.Expression, Expression.Quote(orderByExpression));
                    return this.Source.Provider.CreateQuery<T>(resultExpression);
                }
            }
            catch { }

            return this.Source;
        }

        public IQueryable<T> GetSortedFromList()
        {
            string command = SortingParams.SortDirection == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(T);
            var property = type.GetProperty(SortingParams.OrderBy, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null && !string.IsNullOrEmpty(property.Name))
            {
                var parameter = Expression.Parameter(type, property.Name);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, this.Source.Expression, Expression.Quote(orderByExpression));
                return this.ListSource.AsQueryable().Provider.CreateQuery<T>(resultExpression);
            }

            return this.Source;
        }

    }
}
