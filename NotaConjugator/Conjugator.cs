﻿using NotaConjugator.Data;
using NotaDAL.Context;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator
{
    public class Conjugator
    {
        #region Data Members

        NotaContextAcces context;
        ConjugationPackage conjugationPackage;

        #endregion

        #region Ctor

        public Conjugator(NotaContextAcces context)
        {
            this.context = context;
        }

        #endregion

        #region Methods

        public ConjugationIndex ConjugatePerson(int tenseId, int verbId, int personId)
        {            
            bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

            if (!conjugaitonPackageSuccess)
                return null;

            var conjugationString = Conjugate();

            return new ConjugationIndex
            {
                TenseId = tenseId,
                VerbId = verbId,
                PersonId = personId,
                conjugationString = conjugationString
            };
        }

        public List<ConjugationIndex> ConjugateTense(int tenseId, int verbId)
        {            
            var tenseConjugaitonIndexes = new List<ConjugationIndex>();
            var tensePersonIds = context.GetAllTensePersons(tenseId).Select(p => p.Id);

            foreach (var personId in tensePersonIds)
            {
                bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

                if (!conjugaitonPackageSuccess)
                    continue;

                var conjugationString = Conjugate();

                var conjugation = new ConjugationIndex
                {
                    TenseId = tenseId,
                    VerbId = verbId,
                    PersonId = personId,
                    conjugationString = conjugationString
                };

                tenseConjugaitonIndexes.Add(conjugation);
            }

            return tenseConjugaitonIndexes;
        }

        public List<ConjugationIndex> ConjugateVerb(int verbId, 
                                                    bool enabledTensesOnly = false)
        {
            var tenseConjugaitonIndexes = new List<ConjugationIndex>();
            var tenses = context.GetItemList<Tense>();

            if (enabledTensesOnly)
                tenses = tenses.Where(t => t.Enabled).ToList();

            var tensesIds = tenses.Select(t => t.Id).ToList();
            var _lock = new Object();
            Dictionary<int, List<int>> tensesPersonsIds = context.GetAllTensePersonsIds(tensesIds);            

            Parallel.ForEach(tensesPersonsIds, tensePersonId =>
            {

                var tenseId = tensePersonId.Key;
                var personsIds = tensePersonId.Value;

                Parallel.ForEach(personsIds, personId =>
                {
                    bool conjugaitonPackageSuccess;                    
                    conjugaitonPackageSuccess = buildConjugationPackageSafe(tenseId, verbId, personId, _lock);
                    

                    if (!conjugaitonPackageSuccess)
                        return;

                    var conjugationString = Conjugate();

                    var conjugation = new ConjugationIndex
                    {
                        TenseId = tenseId,
                        VerbId = verbId,
                        PersonId = personId,
                        conjugationString = conjugationString
                    };

                    tenseConjugaitonIndexes.Add(conjugation);
                });
            });

            return tenseConjugaitonIndexes;
        }

        private string Conjugate()
        {
            var conjugationRuleType = conjugationPackage.ConjugationRule.Type;
            var suffix = conjugationPackage.Instruction.Suffix;
            var conjugationString = conjugationPackage.ConjugationMatch.ConjugationString ?? string.Empty;

            switch (conjugationRuleType)
            {
                case ConjugationRuleType.Independent:
                    var pattern = ConjugationUtils.getConjugationMatchPattern(conjugationPackage);
                    return pattern + suffix;
                case ConjugationRuleType.NewPatternDependent:
                    return conjugationString + suffix;
                case ConjugationRuleType.SpecialConjugation:
                    return conjugationString;
                default:
                    throw new Exception("Unexpeted ConjugationRule type");
            }            
        }

        private bool buildConjugationPackage(int tenseId, int verbId, int personId)
        {
            var verb = context.GetItem<Verb>(verbId);            

            if (verb == null)
                return false;

            
            var conjugationMatch = context.getConjugationMatch(tenseId, verbId, personId);
            

            if (conjugationMatch == null)
                return false;

            var conjugationRule = context.getConjugationMatchConjugationRule(conjugationMatch);            

            if (conjugationRule == null)
                return false;
            
            var instruction = context.GetConjugationInstruction(verb, conjugationRule.Id, personId);            

            if (instruction == null)
                return false;

            conjugationPackage = new ConjugationPackage
            {
                Verb = verb,
                ConjugationMatch = conjugationMatch,
                ConjugationRule = conjugationRule,
                Instruction = instruction
            };

            return true;
        }

        private bool buildConjugationPackageSafe(int tenseId, int verbId, int personId, object _lock)
        {
            Verb verb;
            lock (_lock)
            {
                verb = context.GetItem<Verb>(verbId);
            }

            if (verb == null)
                return false;

            ConjugationMatch conjugationMatch;
            lock (_lock)
            {
                conjugationMatch = context.getConjugationMatch(tenseId, verbId, personId);
            }

            if (conjugationMatch == null)
                return false;

            ConjugationRule conjugationRule;
            lock (_lock)
            {
                conjugationRule = context.getConjugationMatchConjugationRule(conjugationMatch);
            }

            if (conjugationRule == null)
                return false;

            ConjugationRulesInstruction instruction;

            lock (_lock)
            {
                instruction = context.GetConjugationInstruction(verb, conjugationRule.Id, personId);
            }

            if (instruction == null)
                return false;

            conjugationPackage = new ConjugationPackage
            {
                Verb = verb,
                ConjugationMatch = conjugationMatch,
                ConjugationRule = conjugationRule,
                Instruction = instruction
            };

            return true;
        }

        #endregion
    }
}
