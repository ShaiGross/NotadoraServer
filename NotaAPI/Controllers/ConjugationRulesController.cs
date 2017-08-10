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
    [RoutePrefix("ConjugationRules")]
    public class ConjugationRulesController : ApiController
    {
        [Route("")]
        public List<ConjugationRuleInfo> GetConjugationRules()
        {
            return DataAccess.GetConjugationRules();
        }
    }
}
