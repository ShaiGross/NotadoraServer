using APIModels;
using NotaBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NotaAPI.Controllers
{
    [RoutePrefix("Tenses")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class TensesController : ApiController
    {
        [Route("")]
        public List<TenseInfo> GetTenses()
        {
            Console.WriteLine("Requested tenses");
            return DataAccess.GetTenses();
        }
    }
}
