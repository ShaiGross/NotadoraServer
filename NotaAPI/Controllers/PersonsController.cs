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
    [RoutePrefix("Persons")]
    public class PersonsController : ApiController
    {
        [Route("")]
        public Dictionary<int, PersonInfo> GetPersons()
        {
            return DataAccess.GetPersons();
        }
    }
}
