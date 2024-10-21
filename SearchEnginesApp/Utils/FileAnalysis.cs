using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Iveonik.Stemmers;


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
        public static Dictionary<string, (int count, List<int> indices)> ExtractKeywordsToDict(string text, int rank = 10, bool stopword = false)
        {
            var wordFrequency = new Dictionary<string, (int count, List<int>indices)>();


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

            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                if(wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word] = (wordFrequency[word].count + 1, wordFrequency[word].indices);
                    // Start Index from One
                    wordFrequency[word].indices.Add(i+1);
                }
                else
                {
                    wordFrequency.Add(word,(1, new List<int> {i+1}));
                }
            }

            // Return All values when get rank equal to zero
            if(rank ==0) { rank = wordFrequency.Count(); }
            return wordFrequency.OrderByDescending(w => w.Value).Take(rank).ToDictionary(w => w.Key, w => w.Value);
        }
        public static Dictionary<string, (int count, List<int> indices)> ExtractKeywordsToDict(List<string> words, int rank = 10, bool stopword = false)
        {
            var wordFrequency = new Dictionary<string, (int count, List<int> indices)>();
            IEnumerable<string> filteredWords;
            if (stopword)
            {
                filteredWords = words.Where(w => w.Length > 2 && !stopWords.Contains(w.ToLower()));
            }
            else
            {
                filteredWords = words.Where(w => w.Length > 2);
            }

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word] = (wordFrequency[word].count + 1, wordFrequency[word].indices);
                    // Start Index from One
                    wordFrequency[word].indices.Add(i + 1);
                }
                else
                {
                    wordFrequency.Add(word, (1, new List<int> { i + 1 }));
                }
            }
            // Return All values when get rank equal to zero
            if (rank == 0) { rank = wordFrequency.Count(); }
            return wordFrequency.OrderByDescending(w => w.Value.count).Take(rank).ToDictionary(w => w.Key, w => w.Value);
        }
        public static List<string> ExtractKeywordsToList(List<string> words, int rank, bool stopword = false)
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
        public static Dictionary<string, Tuple<int, double>> ExtractTokenToDict(string text, bool PortsAlgorithm = false, bool stopword = false)
        {
            Dictionary<string, int> wordFrequency = new Dictionary<string, int>();
            // Tokenize and stem the text
            PortugalStemmer stemmer = new PortugalStemmer();
            string[] tokens = text.Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (stopword)
            {
                tokens = tokens.Where(w => w.Length > 2 && !stopWords.Contains(w)).ToArray();
            }
            else
            {
                tokens = tokens.Where(w => w.Length > 2).ToArray();
            }

            foreach (string token in tokens)
            {
                string stemmedWord = (PortsAlgorithm) ? stemmer.Stem(token) : token;
                if (!wordFrequency.ContainsKey(stemmedWord))
                {
                    wordFrequency[stemmedWord] = 0;
                }
                wordFrequency[stemmedWord]++;
            }

            // Calcalute the total number of words
            int totalWords = wordFrequency.Values.Sum();

            wordFrequency.Values.Sum();

            // Calculate the relative frequency of each word
            Dictionary<string, Tuple<int, double>> wordFrequencyWithRelative = new Dictionary<string, Tuple<int, double>>();
            foreach (var item in wordFrequency)
            {
                double frequency = (double)item.Value / totalWords;
                wordFrequencyWithRelative[item.Key] = new Tuple<int, double>(item.Value, frequency);
            }

            // Sort Word Frequency by Value
            return wordFrequencyWithRelative.OrderByDescending(w => w.Value).ToDictionary(w => w.Key, w => w.Value);
        }
        public static Dictionary<string, Tuple<int, double>> ExtractTokenToDict(List<string> words, bool PortsAlgorithm = false, bool stopword = false)
        {
            Dictionary<string, int> wordFrequency = new Dictionary<string, int>();
            if (stopword)
            {
                words = words.Where(w => w.Length > 2 && !stopWords.Contains(w)).ToList();
            }
            else
            {
                words = words.Where(w => w.Length > 2).ToList();
            }

            // Ports Algorithm
            PortugalStemmer stemmer = new PortugalStemmer();

            foreach (string token in words)
            {
                string stemmedWord = (PortsAlgorithm) ? stemmer.Stem(token): token;
                if (!wordFrequency.ContainsKey(stemmedWord))
                {
                    wordFrequency[stemmedWord] = 0;
                }
                wordFrequency[stemmedWord]++;
            }

            // Calcalute the total number of words
            int totalWords = wordFrequency.Values.Sum();

            wordFrequency.Values.Sum();

            // Calculate the relative frequency of each word
            Dictionary<string, Tuple<int, double>> wordFrequencyWithRelative = new Dictionary<string, Tuple<int, double>>();
            foreach (var item in wordFrequency)
            {
                double frequency = (double)item.Value / totalWords;
                wordFrequencyWithRelative[item.Key] = new Tuple<int, double>(item.Value, frequency);
            }

             // Sort Word Frequency by Value
             return wordFrequencyWithRelative.OrderByDescending(w => w.Value).ToDictionary(w => w.Key, w => w.Value);
        }
    }
}
