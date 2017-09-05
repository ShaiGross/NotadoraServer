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
    public class InstructionInfo
    {                
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private int conjugationRuleId;

        [DataMember]
        private int PersonId;

        [DataMember]
        private VerbType verbType;

        [DataMember]
        private string suffix;            

        #endregion

        #region Ctor

        public InstructionInfo(ConjugationRulesInstruction instruction)
        {
            this.id = instruction.Id;
            this.conjugationRuleId = instruction.ConjugationRuleId;
            this.PersonId  = instruction.PersonId;
            this.verbType = instruction.VerbType;
            this.suffix = instruction.Suffix;
        }

        #endregion
    }
}
