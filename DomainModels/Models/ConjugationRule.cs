﻿using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    public enum ConjugationRuleType
    {
        Independent,        
        NewPatternDependent,        
        SpecialConjugation
    }

    public enum ConjugationPatternType
    {
        None = -1,
        Stem,
        InInf,
        Inf,
        InStem,        
    }

    [Table(Name = "ConjugationRules")]
    public class ConjugationRule : NotaDbObject<ConjugationRule>, IComparable<ConjugationRule>
    {
        #region Properties

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; }

        [Column(CanBeNull = false)]
        public int TenseId { get; set; }

        [Column(DbType = "BIT", CanBeNull = false)]
        public bool IsRegular { get; set; }

        [Column(DbType = "INT")]
        public ConjugationRuleType Type { get; set; }
        
        [Column(CanBeNull = false)]
        public int PersonCount { get; set; }

        [Column(CanBeNull = true)]
        public int? PatternIndex { get; set; }

        internal List<Person> Persons { get; set; }

        #endregion

        #region Expression-Bodied Members

        public ConjugationPatternType PatternType => (!PatternIndex.HasValue) 
                                                     ? ConjugationPatternType.None
                                                     : (PatternIndex < 0)
                                                       ? ConjugationPatternType.InStem
                                                        : (ConjugationPatternType)PatternIndex;

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(ConjugationRule other)
        {
            throw new NotImplementedException();
        }

        public void Copy(ConjugationRule other)
        {
            this.Id = other.Id;
            this.Name = other.Name;
            this.Description = other.Description;
            this.TenseId = other.TenseId;
            this.IsRegular = other.IsRegular;
            this.Type = other.Type;
            this.PersonCount = other.PersonCount;
            this.PatternIndex = other.PatternIndex;
            this.Persons = other.Persons;
        }

        #endregion

        #region IComparable Implementation

        public int CompareTo(ConjugationRule other)
        {
            var personsDiff = this.PersonCount - other.PersonCount;

            if (personsDiff == 0)
            {
                if (this.Type == ConjugationRuleType.SpecialConjugation && other.Type != ConjugationRuleType.SpecialConjugation)
                    return 1;
                else if (this.Type != ConjugationRuleType.SpecialConjugation && other.Type == ConjugationRuleType.SpecialConjugation)
                    return -1;
                else return 0;          
            }

            return personsDiff / Math.Abs(personsDiff);
        }        

        #endregion
    }

}
