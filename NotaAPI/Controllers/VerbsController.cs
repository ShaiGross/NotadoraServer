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

        [Route("{spanishInfinative}")]
        [HttpGet]
        public VerbInfo AddNewVerb(string spanishInfinative)
        {
            return DataAccess.AddNewVerb(spanishInfinative);
        }

        [Route("conjMatches/{id:int}")]
        [HttpGet]
        public List<ConjugationMatchInfo> GetVerbConjugationMatches(int id)
        {
            return DataAccess.GetVerbConjugationMatches(id);
        }

        [Route("conjMatches/{verbId:int}/{tenseId:int}/{personId:int}")]
        [HttpGet]
        public List<ConjugationMatchInfo> GetVerbConjugationMatches(int verbId, int tenseId, int personId)
        {
            return DataAccess.GetVerbConjugationMatches(verbId, tenseId, personId);
        }

        [Route("conjMatches/{verbId:int}/{tenseId:int}/{personId:int}/{conjugationRuleId:int}")]
        [HttpGet]
        public ConjugationMatchInfo GetVerbConjugationMatch(int verbId, int tenseId, int personId, int conjugationRuleId)
        {
            return DataAccess.GetVerbConjugationMatch(verbId, tenseId, personId, conjugationRuleId);
        }
    }
}
