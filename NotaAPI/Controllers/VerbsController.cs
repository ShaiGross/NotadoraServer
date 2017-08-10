using APIModels;
using NotaBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotaAPI.Controllers
{
    [RoutePrefix("verbs")]
    public class VerbsController : ApiController
    {
        [Route("")]
        public List<VerbInfo> GetVerbs(bool conjugate = false)
        {
            return DataAccess.GetVerbs(conjugate);
        }

        [Route("{id:int}")]
        public VerbInfo GetVerbById(int id)
        {
            return DataAccess.GetVerbById(id);
        }
    }
}
