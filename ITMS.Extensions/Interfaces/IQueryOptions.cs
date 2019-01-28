using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITMS.Extensions.Interfaces
{
    public interface IQueryOptions
    {
        string Search { get; set; }
        List<FilterOption> Filters { get; set; }
        List<OrderOption> Orders { get; set; }
        int PageNumber { get; set; }
        int ItemsPerPage { get; set; }
    }
    public class FilterOption
    {
        public string Column { get; set; }
        public string Value { get; set; }
    }
    public class OrderOption
    {
        public string Column { get; set; }
        public bool IsDescending { get; set; }
    }
    public class QueryOptions : IQueryOptions
    {
        public string Search { get; set; }
        public List<FilterOption> Filters { get; set; }
        public List<OrderOption> Orders { get; set; }
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
