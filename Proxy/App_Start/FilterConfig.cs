using Proxy.Filters;
using System.Web;
using System.Web.Http.Filters;

namespace Proxy
{
    public class FilterConfig
    {
        public static void RegisterHttpFilters(HttpFilterCollection filters)
        {
            filters.Add(new CustomFilter());
            filters.Add(new RequreHttpsFilter());
        }
    }
}
