using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIModels
{
    [DataContract]
    public class ConjugationMatchInfo
    {
        #region Properties

        [DataMember]
        private int id;

        [DataMember]
        private int verbId;

        [DataMember]
        private int conjugationRuleId;

        [DataMember]
        private int? personId;

        [DataMember]
        private string conjugationString;

        #endregion

        #region Ctor

        public ConjugationMatchInfo(ConjugationMatch conjMatch)
        {            
            this.id = conjMatch.Id;
            this.verbId = conjMatch.VerbId;
            this.conjugationRuleId = conjMatch.ConjugationRuleId;
            this.personId = conjMatch.PersonId;
            this.conjugationString = conjMatch.ConjugationString;
        }

        #endregion
    }
}
