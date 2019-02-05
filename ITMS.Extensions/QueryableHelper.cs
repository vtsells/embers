using ITMS.Extensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITMS.Extensions
{
    public static class QueryableHelper
    {


        public static IQueryable<TModel> Search<TModel>(this IQueryable<TModel> collection, SearchOption option)
        {
            if (option == null) return collection;
            ParameterExpression pe = Expression.Parameter(typeof(TModel), "m");
            Expression left = Expression.Property(pe, typeof(TModel).GetProperty(option.Column));
            Expression right = Expression.Call(Expression.Constant(option.Value.ToLower(), typeof(string)), typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            Expression contains = Expression.Call(left, method, right);
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { collection.ElementType },
                collection.Expression,
                Expression.Lambda<Func<TModel, bool>>(contains, new ParameterExpression[] { pe })
                );
            return collection.Provider.CreateQuery<TModel>(whereCallExpression);

        }
        public static IQueryable<TModel> Page<TModel>(this IQueryable<TModel> collection, PageOption option)
        {
            if (option == null) return collection;
            return collection.Skip((option.PageNumber - 1) * option.PageLength).Take(option.PageLength);
        }
        public static IQueryable<TModel> Order<TModel>(this IQueryable<TModel> collection, IEnumerable<OrderOption> options)
        {
            if (options == null) return collection;
            IOrderedQueryable<TModel> qOrdered;
            var option = options.FirstOrDefault();
            if (option.IsDescending)
                qOrdered = collection.OrderByDescending(option.Column);
            else
                qOrdered = collection.OrderBy(option.Column);
            if (options.Count() == 1)
                return qOrdered;
            for (int i = 1; i < options.Count(); i++)
            {
                option = options.ElementAt(i);
                if (option.IsDescending)
                    qOrdered = qOrdered.ThenByDescending(option.Column);
                else
                    qOrdered = qOrdered.ThenBy(option.Column);
            }
            return qOrdered;
        }
        public static IQueryable<TModel> Filter<TModel>(this IQueryable<TModel> collection, IEnumerable<FilterOption> options)
        {
            if (options == null) return collection;
            foreach (var option in options)
            {
                collection = collection.Where(option);
            }
            return collection;
        }
        public static async Task<IEnumerable<TModel>> ApplyQueryAsync<TModel>(this IQueryable<TModel> collection, IQueryOptions options) =>
            await collection.Search(options.Search).Filter(options.Filters).Order(options.Orders).Page(options.Page).ToListAsync();
        
        #region Helpers
        public static IOrderedQueryable<TModel> OrderBy<TModel>(this IQueryable<TModel> q, string name)
        {
            Type entityType = typeof(TModel);
            PropertyInfo p = entityType.GetProperty(name);
            MethodInfo m = typeof(QueryableHelper).GetMethod("OrderByProperty").MakeGenericMethod(entityType, p.PropertyType);
            return (IOrderedQueryable<TModel>)m.Invoke(null, new object[] { q, p });
        }
        public static IOrderedQueryable<TModel> OrderByDescending<TModel>(this IQueryable<TModel> q, string name)
        {
            Type entityType = typeof(TModel);
            PropertyInfo p = entityType.GetProperty(name);
            MethodInfo m = typeof(QueryableHelper).GetMethod("OrderByPropertyDescending").MakeGenericMethod(entityType, p.PropertyType);
            return (IOrderedQueryable<TModel>)m.Invoke(null, new object[] { q, p });
        }
        public static IOrderedQueryable<TModel> OrderByPropertyDescending<TModel, TRet>(IQueryable<TModel> q, PropertyInfo p)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.OrderByDescending(Expression.Lambda<Func<TModel, TRet>>(se, pe));
        }
        public static IOrderedQueryable<TModel> OrderByProperty<TModel, TRet>(IQueryable<TModel> q, PropertyInfo p)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.OrderBy(Expression.Lambda<Func<TModel, TRet>>(se, pe));
        }
        public static IOrderedQueryable<TModel> ThenBy<TModel>(this IQueryable<TModel> q, string name)
        {
            Type entityType = typeof(TModel);
            PropertyInfo p = entityType.GetProperty(name);
            MethodInfo m = typeof(QueryableHelper).GetMethod("ThenByProperty").MakeGenericMethod(entityType, p.PropertyType);
            return (IOrderedQueryable<TModel>)m.Invoke(null, new object[] { q, p });
        }
        public static IOrderedQueryable<TModel> ThenByDescending<TModel>(this IQueryable<TModel> q, string name)
        {
            Type entityType = typeof(TModel);
            PropertyInfo p = entityType.GetProperty(name);
            MethodInfo m = typeof(QueryableHelper).GetMethod("ThenByPropertyDescending").MakeGenericMethod(entityType, p.PropertyType);
            return (IOrderedQueryable<TModel>)m.Invoke(null, new object[] { q, p });
        }
        public static IOrderedQueryable<TModel> ThenByPropertyDescending<TModel, TRet>(IOrderedQueryable<TModel> q, PropertyInfo p)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.ThenByDescending(Expression.Lambda<Func<TModel, TRet>>(se, pe));
        }
        public static IOrderedQueryable<TModel> ThenByProperty<TModel, TRet>(IOrderedQueryable<TModel> q, PropertyInfo p)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.ThenBy(Expression.Lambda<Func<TModel, TRet>>(se, pe));
        }
        public static IQueryable<TModel> Where<TModel>(this IQueryable<TModel> q, FilterOption option)
        {

            ParameterExpression pe = Expression.Parameter(typeof(TModel), "m");
            Expression left = Expression.Property(pe, typeof(TModel).GetProperty(option.Column));
            Expression right = Expression.Call(Expression.Constant(option.Value.ToLower(), typeof(string)), typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            Expression eq = Expression.Equal(left, right);
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { q.ElementType },
                q.Expression,
                Expression.Lambda<Func<TModel, bool>>(eq, new ParameterExpression[] { pe })
                );
            return q.Provider.CreateQuery<TModel>(whereCallExpression);
        }
        #endregion


    }
}

