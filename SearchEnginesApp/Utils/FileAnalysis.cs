using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchEnginesApp.Utils
{
    public class KeywordExtractor
    {
        private static HashSet<string> stopWords = new HashSet<string>
        {
            "the", "is", "are", "a", "an", "and", "or", "but", "if", "then", "else", 
            "when", "at", "by", "for", "with", "about", "against", "between", "into", 
            "through", "during", "before", "after", "above", "below", "to", "from", 
            "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", 
            "then", "once", "here", "there", "where", "why", "how", "all", "any", "both", 
            "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", 
            "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", 
            "just", "don", "should", "now","this","that","was", "were","start","end",
            "data", "have", "these",
        };
        public static Dictionary<string, int> ExtractKeywordsToDict(string text, int rank = 10, bool stopword = true)
        {
            var wordFrequency = new Dictionary<string, int>();


            // Spilt text to word list 
            var words = Regex.Split(text.ToLower(), @"\W+");
            // Check StopWord operation
            if (stopword)
            {
                words = words.Where(w => w.Length > 2 && !stopWords.Contains(w)).ToArray();
            }
            else
            {
                words = words.Where(w => w.Length > 2).ToArray();
            }


            foreach (var word in words)
            {
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency.Add(word, 1);
                }
            }
            // Return All values when get rank equal to zero
            if(rank ==0) { rank = wordFrequency.Count(); }
            return wordFrequency.OrderByDescending(w => w.Value).Take(rank).ToDictionary(w => w.Key, w => w.Value);
        }
        public static Dictionary<string, int> ExtractKeywordsToDict(List<string> words, int rank = 10, bool stopword = true)
        {
            var wordFrequency = new Dictionary<string, int>();
            IEnumerable<string> filteredWords;
            if (stopword)
            {
                filteredWords = words.Where(w => w.Length > 2 && !stopWords.Contains(w.ToLower()));
            }
            else
            {
                filteredWords = words.Where(w => w.Length > 2);
            }

            foreach (var word in filteredWords)
            {
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency.Add(word, 1);
                }
            }
            // Return All values when get rank equal to zero
            if (rank == 0) { rank = wordFrequency.Count(); }
            return wordFrequency.OrderByDescending(w => w.Value).Take(rank).ToDictionary(w => w.Key, w => w.Value);
        }
        public static List<string> ExtractKeywordsToList(List<string> words, int rank, bool stopword = true)
        {
            var wordFrequency = new Dictionary<string, int>();
            IEnumerable<string> filteredWords;
            if (stopword)
            {
                filteredWords = words.Where(w => w.Length > 2 && !stopWords.Contains(w.ToLower()));
            }
            else
            {
                filteredWords = words.Where(w => w.Length > 2);
            }

            foreach (var word in filteredWords)
            {
                string lowerWord = word.ToLower();
                if (wordFrequency.ContainsKey(lowerWord))
                {
                    wordFrequency[lowerWord]++;
                }
                else
                {
                    wordFrequency.Add(lowerWord, 1);
                }
            }

            List<string> result = wordFrequency.OrderByDescending(w => w.Value)
                                .Take(rank)
                                .Select(w => w.Key)
                                .ToList();
            return result.Select(word=>word.ToUpper()).ToList();
        }
    }
}
