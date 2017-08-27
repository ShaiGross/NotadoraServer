using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConjugationsIngestor
{
    public class ConjugationParser
    {
        #region Consts

        private const string MAX_PERSON_PER_TENSE_APP_KEY = "MAX_PERSON_PER_KEY";

        #endregion

        #region Methods

        public Dictionary<Tense, List<string>> ParseHTML(ref string html,
                                                         out string englishInf,
                                                         out string presentParticiple,
                                                         out string pastParticiple,                                                         
                                                         List<Tense> allTenses)
        {
            englishInf = ParseEnglishInfinative(ref html);
            presentParticiple = ParsePresentParticiple(ref html);
            pastParticiple = ParsePastParticiple(ref html);
            
            var tensesConjugations = SortTenseList(allTenses, pastParticiple, presentParticiple);

            ParseTenses(html, tensesConjugations);            

            return tensesConjugations;
        }

        private string ParseEnglishInfinative(ref string html)
        {
            var englishInfLeftIdentifier = "<div class=\"el\">";
            var englishInfRightIdentifier = "</div>";

            var englishInfLeftIndex = html.IndexOf(englishInfLeftIdentifier) + englishInfLeftIdentifier.Length;
            html = html.Remove(0, englishInfLeftIndex);

            var englishInfRightIndex = html.IndexOf(englishInfRightIdentifier);

            var LinksHtml = html.Substring(0, englishInfRightIndex);
            var inEnglishInf = true;
            var englishInf = string.Empty;

            foreach (var currChar in LinksHtml)
            {
                if (currChar == '<')
                    inEnglishInf = false;
                else if (currChar == '>')
                    inEnglishInf = true;
                else if (inEnglishInf)
                    englishInf += currChar;
            }

            var comaIndex = englishInf.IndexOf(',');
            if (comaIndex > 0)
                englishInf = englishInf.Remove(comaIndex);

            return englishInf.TrimEnd();
        }

        private Dictionary<Tense, List<string>> SortTenseList(List<Tense> unsortedTenses,
                                                              string pastParticiple,
                                                              string presentParticiple)
        {
            var sortedTenses = new Dictionary<Tense, List<string>>(unsortedTenses.Count);
            Tense currTense;

            try
            {                
                currTense = unsortedTenses.First(t => t.Name == "Present");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Preterite");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Imperfect");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Conditional");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Future");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Past Participle");
                sortedTenses.Add(currTense,
                                 new List<string>(currTense.PersonsCount));
                sortedTenses[currTense].Add(pastParticiple);

                currTense = unsortedTenses.First(t => t.Name == "Present Participle");
                sortedTenses.Add(currTense,
                                 new List<string>(currTense.PersonsCount));
                sortedTenses[currTense].Add(presentParticiple);

            }
            catch
            {
                return null;
            }
            
            return sortedTenses;
        }

        private static string ParsePresentParticiple(ref string html)
        {
            var presentParticipleElement = "Present Participle:</a>";
            var presentParticipleLabelIndex = html.IndexOf(presentParticipleElement);
            html = html.Remove(0, presentParticipleLabelIndex + presentParticipleElement.Length);
            
            var presentParticipleLeftBorder = "\"conj-basic-word\">";
            var presentParticipleLeft = html.IndexOf(presentParticipleLeftBorder) + presentParticipleLeftBorder.Length;
            html = html.Remove(0, presentParticipleLeft);
            var presentParticipleRightBorder = html.IndexOf("</span>");

            var presentParticiple = html.Substring(0, presentParticipleRightBorder);

            if (presentParticiple.Contains(','))
                presentParticiple = presentParticiple.Remove(presentParticiple.IndexOf(','));

            FixAccentedLatters(ref presentParticiple);

            return presentParticiple;
        }

        private static string ParsePastParticiple(ref string html)
        {            
            var participleElement = "Participle:</a>";
            var gerundIndex = html.IndexOf(participleElement);
            html = html.Remove(0, gerundIndex + participleElement.Length);

            var pastParticipleLeftBorder = "\"conj-basic-word\">";
            var pastParticipleLeft = html.IndexOf(pastParticipleLeftBorder) + pastParticipleLeftBorder.Length;
            html = html.Substring(pastParticipleLeft);
            var pastParticipleRigth = html.IndexOf("</span>");

            var pastParticiple = html.Substring(0, pastParticipleRigth);

            if (pastParticiple.Contains(','))
                pastParticiple = pastParticiple.Remove(pastParticiple.IndexOf(','));

            FixAccentedLatters(ref pastParticiple);

            return pastParticiple;
        }

        private static void ParseTenses(string html, Dictionary<Tense, List<string>> tensesConjugations)
        {
            // TODO: Fix this into config libreary
            var maxPersonPerTenses = int.Parse(ConfigurationManager.AppSettings[MAX_PERSON_PER_TENSE_APP_KEY]);
            var tableElement = "<table class=\"vtable\">";
            var tableIndex = html.IndexOf(tableElement);
            html = html.Remove(0, tableIndex + tableElement.Length);

            var tableLeft = html.IndexOf("</tr><tr>") + "</tr>".Length;
            var tableRigth = html.IndexOf("</table>");

            html = html.Substring(tableLeft, tableRigth - tableLeft).Replace(" ", "");
            var tenses = tensesConjugations.Keys.Where(t => !t.Name.Contains("Participle")).ToList();

            for (int trIndex = 0; trIndex < maxPersonPerTenses; trIndex++)
            {
                var conjugationWrapperLeftBorder = "<divdata-toggle=\"tooltip\"";
                var conjugationWrapperLeft = html.IndexOf(conjugationWrapperLeftBorder);
                      
                html = html.Remove(0, conjugationWrapperLeft);
                html = html.Replace("<spanclass='conj-irregular'>", "").Replace("</span>", "");                

                for (int tenseIndex = 0; tenseIndex < tenses.Count; tenseIndex++)
                {
                    var currTense = tenses[tenseIndex];
                    var tdLeft = html.IndexOf("\">") + "\">".Length;
                    var tdRight = html.IndexOf("</div>");
                    var conjugatedGrammPerson = html.Substring(tdLeft, tdRight - tdLeft);
                    html = html.Substring(tdRight);

                    if (conjugatedGrammPerson.Contains('>'))
                        conjugatedGrammPerson = conjugatedGrammPerson.Substring(conjugatedGrammPerson.IndexOf('>') + 1);

                    conjugatedGrammPerson = conjugatedGrammPerson.Trim();
                    FixAccentedLatters(ref conjugatedGrammPerson);

                    if (tenseIndex + 1 != tenses.Count || trIndex != 5)
                    {                        
                        conjugationWrapperLeft = html.IndexOf(conjugationWrapperLeftBorder);
                        html = html.Remove(0, conjugationWrapperLeft);
                    }

                    tensesConjugations[currTense].Add(conjugatedGrammPerson);
                }
            }

            foreach (var currTense in tenses)
            {
                tensesConjugations[currTense][4] = tensesConjugations[currTense][5];
            }
        }

        private static void FixAccentedLatters(ref string word)
        {
            word = (word.Replace("Ã¡", "á").
                         Replace("Ã©", "é").
                         Replace("Ã­", "í").
                         Replace("Ã³", "ó").
                         Replace("Ãº", "ú").
                         Replace("Ã±", "ñ"));
        }

        #endregion
    }
}
