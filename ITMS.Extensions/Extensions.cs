using ITMS.Extensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITMS.Extensions
{
    public static class ObjectExtensions
    {
        public static bool Search(this object ob, string search)
        {
            var type = ob.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {

                var val = prop.GetValue(ob, null);
                if (val.ToString().Contains(search)) return true;
            }
            return false;
        }
        public static bool Filter(this object ob, List<FilterOption> filters)
        {

            var type = ob.GetType();
            var found = true;
            foreach (var filter in filters)
            {
                var foundColumn = type.GetProperty(filter.Column)?.Name;
                if (foundColumn != null)
                {
                    if (!type.GetProperty(filter.Column).GetValue(ob, null).ToString().Contains(filter.Value))
                    {
                        found = false;
                    }
                }
                else
                {
                    found = false;
                }
            }
            return found;
        }
        public static IQueryable<object> Order (this IQueryable<object> ob,List<OrderOption> orders)
        {
            if (orders.Count() == 1)
            {
                return ob.OrderBy(m => m.GetPropertyValue(orders.FirstOrDefault().Column));
            }
            throw new NotImplementedException();
        }
        public static string GetProperty(this object ob, string name)
        {
            var type = ob.GetType();
            var property = type.GetProperty(name)?.Name;
            return property;
        }
        public static string GetPropertyValue(this object ob, string name)
        {
            var type = ob.GetType();
            var property = type.GetProperty(name);
            return property.GetValue(ob,null).ToString();
        }
        public static IQueryable<object> Page(this IQueryable<object> ob, int pageNumber, int itemsPerPage)
        {
            return ob.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage);
        }
        public static IQueryable<object> ApplySearch(this IQueryable<object> models, IQueryOptions options)
        {
            return models.Where(m => m.Search(options.Search));
        }
        public static IQueryable<object> ApplyFilters(this IQueryable<object> models, IQueryOptions options)
        {
            return models.Where(m => m.Filter(options.Filters));
        }
        public static IQueryable<object> ApplyOrders(this IQueryable<object> models, IQueryOptions options)
        {
            return models.Order(options.Orders);
        }

        public static IQueryable<object> ApplyPageNumber(this IQueryable<object> models, IQueryOptions options)
        {
            return  models.Page(options.PageNumber, options.ItemsPerPage);
        }

        public static IQueryable<object> ApplyQuery(this IQueryable<object> models, IQueryOptions options)
        {
            var query = models.ApplySearch(options).ApplyFilters(options).ApplyOrders(options).ApplyPageNumber(options);
            return query;
        }
        public static List<T> ListOfType<T>(this T type)
        {
            return new List<T>();
        }

    }
}
