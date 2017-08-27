using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using NotaDAL.Models;

namespace APIModels
{
    [DataContract]
    public class ConjugationRuleInfo
    {
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private string name;

        [DataMember]
        private string description;

        [DataMember]
        private int tenseId;

        [DataMember]
        private bool isRegular;

        [DataMember]
        private ConjugationRuleType conjugationRuleType;

        [DataMember]
        private int? patternIndex;

        [DataMember]
        private List<int> personsIds;

        [DataMember]
        private List<int> verbsIds;

        #endregion

        #region Ctor

        public ConjugationRuleInfo(int id
                                   ,string name
                                   ,string desc
                                   ,int tenseId
                                   ,bool regular
                                   ,ConjugationRuleType type
                                   ,int? patternIndex
                                   ,List<int> personsIds = null
                                   ,List<int> verbIds = null)
        {
            this.id = id;
            this.name = name;
            this.description = desc;
            this.tenseId = tenseId;
            this.isRegular = regular;
            this.conjugationRuleType = type;
            this.patternIndex = patternIndex;
            this.personsIds = personsIds;
            this.verbsIds = verbIds;
        }

        #endregion

        #region Methods

        public ConjugationRule ToDbType() // TODO: this should be interface
        {
            return new ConjugationRule
            {
                Id = this.id
                ,Description = this.description
                ,IsRegular = this.isRegular
                ,Name = this.name
                ,PatternIndex = this.patternIndex
                ,PersonCount = this.personsIds.Count
                ,TenseId = this.tenseId
                ,Type = this.conjugationRuleType
            };
        }

        #endregion
    }
}
