using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MovieAppWeb.Web.Areas.Admin
{
    public class AdminAreaRegistration
    {
        public static void RegisterArea(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "Admin",
                pattern: "Admin/{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });
        }
    }
} 