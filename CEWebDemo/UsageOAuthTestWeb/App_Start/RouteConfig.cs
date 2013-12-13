using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SageNA.CE.MvcApplicationSignOnDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // set the default to action = "DemoContent" to test automatic authentication based on UMAuthorize attribute / MVC authorization check
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "CEWebDemo", action = "DemoContent", id = UrlParameter.Optional }
            );
        }
    }
}