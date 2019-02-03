using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITMS.Extensions.Interfaces
{
    public interface IQueryOptions
    {
        SearchOption Search { get; set; }
        List<FilterOption> Filters { get; set; }
        List<OrderOption> Orders { get; set; }
        PageOption Page { get; set; }
    }
    public class SearchOption {
        public string Column { get; set; }
        public string Value { get; set; }
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
    public class PageOption
    {
        public int PageNumber { get; set; }
        public int PageLength { get; set; }
    }
    public class QueryOptions : IQueryOptions
    {
        public SearchOption Search { get; set; }
        public List<FilterOption> Filters { get; set; }
        public List<OrderOption> Orders { get; set; }
        public PageOption Page { get; set; }
    }
}
