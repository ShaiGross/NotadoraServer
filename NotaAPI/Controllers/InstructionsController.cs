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
    [RoutePrefix("Instructions")]
    public class InstructionsController : ApiController
    {
        [Route("")]
        public List<InstructionInfo> GetInstrctions()
        {
            return DataAccess.GetConjugationRulesInstructions();
        }
    }
}
