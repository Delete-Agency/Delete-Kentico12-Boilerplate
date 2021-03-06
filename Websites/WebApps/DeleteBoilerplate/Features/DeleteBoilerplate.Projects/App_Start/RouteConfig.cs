﻿using System.Web.Mvc;
using System.Web.Routing;

namespace DeleteBoilerplate.Projects
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Project Search by Area",
                url: "projectsearch/area",
                defaults: new { controller = "Projects", action = "SearchByArea" }
            );

            routes.MapRoute(
                name: "Project Search",
                url: "projectsearch/{year}",
                defaults: new { controller = "Projects", action = "Search" }
            );

        }
    }
}
