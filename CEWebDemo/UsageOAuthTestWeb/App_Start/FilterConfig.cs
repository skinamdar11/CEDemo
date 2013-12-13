using System.Web;
using System.Web.Mvc;

namespace SageNA.CE.MvcApplicationSignOnDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}