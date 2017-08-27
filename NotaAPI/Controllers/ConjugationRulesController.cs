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

        [Route("{id:int}")]
        public ConjugationRuleInfo GetConjugationRulebById(int id)
        {
            return DataAccess.GetConjugationRule(id);
        }

        [Route("")]
        [HttpPost]
        public ConjugationRuleInfo UpdateConjugationRule([FromBody]ConjugationRuleInfo conjugationRule)
        {
            if (conjugationRule == null)
                return null;

            return DataAccess.UpdateItem(conjugationRule);
        }
    }
}
