using APIModels;
using ConjugationsIngestor;
using NotaConjugator;
using NotaConjugator.Data;
using NotaDAL.Context;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaBL
{
    public static class DataAccess
    {
        #region Methods

        public static List<VerbInfo> GetVerbs(bool conjugate = false)
        {
            var verbs = new List<VerbInfo>();
            VerbInfo verb = null;

            using (var context = new NotaContextAcces())
            {
                Dictionary<int, List<int>> allVerbsConjugationRulesIds = null;


                if (!conjugate)
                    allVerbsConjugationRulesIds = context.GetAllVerbsConjugationRulesIds();

                var dbVerbs = context.GetItemList<Verb>();

                foreach (var dbVerb in dbVerbs)
                {
                    List<int> conjugationRulesIds = null;


                    if (!conjugate)
                    {
                        if (allVerbsConjugationRulesIds.ContainsKey(dbVerb.Id))
                            conjugationRulesIds = allVerbsConjugationRulesIds[dbVerb.Id];

                        verb = new VerbInfo(dbVerb.Id
                                            , dbVerb.Description
                                            , dbVerb.Infinative
                                            , dbVerb.EnglishInfinative
                                            , null
                                            , conjugationRulesIds);
                    }
                    else
                    {
                        verb = createVerbWithConjugations(dbVerb, context);
                    }

                    verbs.Add(verb);
                }
            }

            return verbs;
        }

        public static VerbInfo AddNewVerb(string spanishInfinative)
        {
            var conjugationHTML = ConjugationsReader.DownloadConjugationsHTML(spanishInfinative);

            if (string.IsNullOrEmpty(conjugationHTML))
                return null;

            var parser = new ConjugationParser();
            var dbContext = new NotaContextAcces();                        
            var allTenses = dbContext.GetItemList<Tense>();
            var tensesConjugations = parser.ParseHTML(ref conjugationHTML
                                                         ,out var enlishInf
                                                         ,out var presentParticiple
                                                         ,out var pastParticiple
                                                         ,allTenses);            

            if (tensesConjugations == null || !tensesConjugations.Any())
                return null;

            var verb = new Verb(spanishInfinative, enlishInf, "needs description");
            dbContext.AddItem(verb);

            var conjugationsClassifier = new ConjugationsClassifier(dbContext);
            conjugationsClassifier.ClassifyVerbConjugators(verb, tensesConjugations);

            dbContext.Dispose();

            return GetVerbById(verb.Id);
            //return GetVerbById(verb.Id);
        }

        public static ConjugationRuleInfo UpdateItem(ConjugationRuleInfo conjugationRule)
        {
            using (var context = new NotaContextAcces())
            {
                var dbConjugationRule = conjugationRule.ToDbType();

                var newDbConjugationRule = context.UpdateItem<ConjugationRule>(dbConjugationRule);

                if (newDbConjugationRule == null)
                    return null;

                return conjugationRule;
            }
        }

        public static ConjugationRuleInfo GetConjugationRule(int id)
        {
            ConjugationRuleInfo conjugationRule = null;

            using (var context = new NotaContextAcces())
            {
                var dbConjugationRule = context.GetItem<ConjugationRule>(id);

                var conjugationRulePersonsIds = context.GetConjugationRulePersons(dbConjugationRule)
                                                       .Select(p => p.Id)
                                                       .ToList();

                var conjugationRulesVerbsIds = context.GetConjugationRuleVerbsIds(dbConjugationRule);


                conjugationRule = new ConjugationRuleInfo(dbConjugationRule.Id
                                                              ,dbConjugationRule.Name
                                                              ,dbConjugationRule.Description
                                                              ,dbConjugationRule.TenseId
                                                              ,dbConjugationRule.IsRegular
                                                              ,dbConjugationRule.Type
                                                              ,dbConjugationRule.PatternIndex
                                                              ,conjugationRulePersonsIds
                                                              ,conjugationRulesVerbsIds);
            }

            return conjugationRule;
        }

        private static VerbInfo createVerbWithConjugations(Verb dbVerb, NotaContextAcces context)
        {
            var conjugator = new Conjugator(context);
            var conjugationIndexes = conjugator.ConjugateVerb(dbVerb.Id, true);
            var verbConjugations = CastAndSortConjugationIndexes(dbVerb.Id, conjugationIndexes);
            var verbConjugationRulesIds = context.GetVerbConjugationRulesIds(dbVerb);

            return new VerbInfo(dbVerb.Id,
                                dbVerb.Description,
                                dbVerb.Infinative,
                                dbVerb.EnglishInfinative,
                                verbConjugations,
                                verbConjugationRulesIds);
        }

        public static VerbInfo GetVerbById(int verbId)
        {
            using (var context = new NotaContextAcces())
            {
                var dbVerb = context.GetItem<Verb>(verbId);
                return createVerbWithConjugations(dbVerb, context);
            }
        }

        public static List<ConjugationRuleInfo> GetConjugationRules()
        {
            var conjugationRules = new List<ConjugationRuleInfo>();

            using (var context = new NotaContextAcces())
            {
                var dbConjugationRules = context.GetItemList<ConjugationRule>();

                foreach (var dbConjugationRule in dbConjugationRules)
                {                    
                    var conjugationRule = new ConjugationRuleInfo(dbConjugationRule.Id
                                                                  ,dbConjugationRule.Name
                                                                  ,dbConjugationRule.Description
                                                                  ,dbConjugationRule.TenseId
                                                                  ,dbConjugationRule.IsRegular
                                                                  ,dbConjugationRule.Type
                                                                  ,dbConjugationRule.PatternIndex);

                    conjugationRules.Add(conjugationRule);
                }
            }

            return conjugationRules;
        }

        public static List<PersonInfo> GetPersons()
        {
            var persons = new List<PersonInfo>();

            using (var context = new NotaContextAcces())
            {
                var dbPersons = context.GetItemList<Person>();

                foreach (var dbPerson in dbPersons)
                {

                    var person = new PersonInfo(dbPerson.Id,
                                              dbPerson.Description,
                                              dbPerson.SpanishExpression,
                                              dbPerson.Plurality,
                                              dbPerson.Formality,
                                              dbPerson.Gender,
                                              dbPerson.Order);
                    persons.Add(person);
                }
            }

            return persons;
        }

        public static List<TenseInfo> GetTenses()
        {
            var tenses = new List<TenseInfo>();
            using (var context = new NotaContextAcces())
            {
                var dbTenses = context.GetItemList<Tense>(t => t.Enabled);
                var conjugator = new Conjugator(context);

                foreach (var dbTense in dbTenses)
                {
                    //var irregularConjugationRulesIds = context.GetTenseIrregularConjugationRules(dbTense)
                    //                                          .Select(cr => cr.Id)
                    //                                          .ToList();

                    //var tensePersonsIds = context.GetAllTensePersons(dbTense.Id)
                    //                             .Select(p => p.Id)
                    //                             .ToList();

                    var tense = new TenseInfo(dbTense.Id
                                              , dbTense.Name
                                              , dbTense.Description
                                              , dbTense.RugularConjugationRuleId
                                              //,irregularConjugationRulesIds
                                              /*,tensePersonsIds*/);
                    tenses.Add(tense);
                }
            }

            return tenses;
        }

        #endregion

        #region Casting Methods

        private static List<VerbConjugations> CastAndSortConjugationIndexes(int verbId, List<ConjugationIndex> conjugationIndexes)
        {
            var verbConjugations = new List<VerbConjugations>(conjugationIndexes.Count);
            conjugationIndexes = conjugationIndexes.OrderBy(ci => ci.TenseId).ToList();

            foreach (var conjugationIndex in conjugationIndexes)
            {
                var personId = conjugationIndex.PersonId;
                var tenseId = conjugationIndex.TenseId;
                var conjugation = conjugationIndex.conjugationString;

                var verbConjugation = new VerbConjugations(tenseId,
                                                           personId,
                                                           conjugation);
                verbConjugations.Add(verbConjugation);
            }

            return verbConjugations;
        }

        #endregion
    }
}