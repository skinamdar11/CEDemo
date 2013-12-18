using System.Web.Mvc;

namespace WebApplication.Areas.API
{
    public class APIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "API";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "API_default",
                "API/{controller}/{action}",
                new { controller = "HelloWorld", action = "Index"}
            );
        }
    }
}
