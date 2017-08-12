using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NotaAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {            
            // Web API routes
            config.MapHttpAttributeRoutes();

            var allowCorsSetting = System.Configuration.ConfigurationManager.AppSettings["allowCors"];
            bool allowCORS = false;

            bool.TryParse(allowCorsSetting, out allowCORS);

            if (allowCORS)
            {
                var cors = new EnableCorsAttribute("*", "*", "*");
                config.EnableCors(cors);
            }

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );            

            config.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };
        }
    }
}
