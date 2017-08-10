using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
//using System.Web.Http.Cors;

namespace NotaAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {            
            // Web API routes
            config.MapHttpAttributeRoutes();

            //var cors = new EnableCorsAttribute("*", "*", "*");
            // Web API configuration and services
            //config.EnableCors(cors);

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
