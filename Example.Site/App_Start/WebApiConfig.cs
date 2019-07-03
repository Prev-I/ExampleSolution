using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Example.Site.Filters;


namespace Example.Site
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Servizi e configurazione dell'API Web

            // Route dell'API Web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //TODO: Check if is ready to be enabled on site
            //config.Filters.Add(new SecureChannelFilter());
        }
    }
}
