using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using SpellCheck_Deletions.Utilities;

namespace SpellCheck_Deletions
{
    public class SpellChecker
    {
        const string BG_DICTIONARY_PATH = @"/Dictionaries/bg_frequency_dictionary.txt";

        public Dictionary<string, Int32> bgDictionary;
        public static int verbose = 0;

        public static int maxWordLength = 0;
        public static int prefixLength = 7;

        public SpellChecker()
        {
            this.InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            if (this.bgDictionary == null || this.bgDictionary.Count == 0)
            {
                var bgDictionaryFilePath = Directory.GetCurrentDirectory() + BG_DICTIONARY_PATH;
                this.bgDictionary = DictionaryGenerator.LoadDictionary(bgDictionaryFilePath, "bg", 0, 1);
            }
        }

        public List<string> Correct(string input, string language, int editDistanceMax)
        {
            if (language == "bg")
            {
                return this.LookupItemInDictionary(this.bgDictionary, input, editDistanceMax);
            }

            return new List<string>();
        }
              
        private List<string> LookupItemInDictionary(Dictionary<string, int> dictionary, string input, int editDistanceMax)
        {
            List<string> candidates = new List<string>();
            HashSet<string> deletesHashset = new HashSet<string>();

            List<SuggestedItem> suggestions = new List<SuggestedItem>();
            HashSet<string> suggestionsHashset = new HashSet<string>();

            int initialEditDistanceMax = editDistanceMax;

            int candidatePointer = 0;

            candidates.Add(input);
            while (candidatePointer < candidates.Count)
            {
                string candidate = candidates[candidatePointer++];
                int lengthDiff = Math.Min(input.Length, prefixLength) - candidate.Length;

                if ((verbose < 2) && (suggestions.Count > 0) && (lengthDiff > suggestions[0].distance))
                {
                    return this.GetSortedList(suggestions);
                }

                int value;
                if (dictionary.TryGetValue(candidate, out value))
                {
                    var inputItem = new InputItem();
                    if (value >= 0)
                    {
                        inputItem.suggestions.Add((Int32)value);
                    }
                    else
                    {
                        inputItem = DictionaryGenerator.GetItemsList()[-value - 1];
                    }

                    if (inputItem.appearances > 0)
                    {
                        int distance = input.Length - candidate.Length;

                        if ((distance <= editDistanceMax)
                            && ((verbose == 2) || (suggestions.Count == 0) || (distance <= suggestions[0].distance))
                            && (suggestionsHashset.Add(candidate)))
                        {
                            if ((verbose < 2) && (suggestions.Count > 0) && (suggestions[0].distance > distance))
                                suggestions.Clear();

                            var suggestedItem = new SuggestedItem()
                            {
                                word = candidate,
                                count = inputItem.appearances,
                                distance = distance
                            };
                            suggestions.Add(suggestedItem);

                            if ((verbose < 2) && (distance == 0))
                            {
                                return this.GetSortedList(suggestions);
                            }
                        }
                    }

                    foreach (int suggestionInt in inputItem.suggestions)
                    {
                        string suggestion = DictionaryGenerator.GetUniqueWordList()[suggestionInt];

                        int distance = 0;
                        if (suggestion != input)
                        {
                            int min = 0;
                            if (Math.Abs(suggestion.Length - input.Length) > initialEditDistanceMax)
                            {
                                continue;
                            }
                            else if (candidate.Length == 0)
                            {
                                if (!suggestionsHashset.Add(suggestion))
                                    continue;

                                distance = Math.Max(input.Length, suggestion.Length);
                            }
                            else
                                if ((prefixLength - editDistanceMax == candidate.Length) && (((min = Math.Min(input.Length, suggestion.Length) - prefixLength) > 1) && (input.Substring(input.Length + 1 - min) != suggestion.Substring(suggestion.Length + 1 - min))) || ((min > 0) && (input[input.Length - min] != suggestion[suggestion.Length - min]) && ((input[input.Length - min - 1] != suggestion[suggestion.Length - min]) || (input[input.Length - min] != suggestion[suggestion.Length - min - 1]))))
                                {
                                    continue;
                                }
                                else
                                    if ((suggestion.Length == candidate.Length) && (input.Length <= prefixLength))
                                    {
                                        if (!suggestionsHashset.Add(suggestion))
                                            continue;

                                        distance = input.Length - candidate.Length;
                                    }
                                    else if ((input.Length == candidate.Length) && (suggestion.Length <= prefixLength))
                                    {
                                        if (!suggestionsHashset.Add(suggestion))
                                            continue;

                                        distance = suggestion.Length - candidate.Length;
                                    }
                                    else if (suggestionsHashset.Add(suggestion))
                                    {
                                        distance = EditDistance.DamerauLevenshteinDistance(input, suggestion, initialEditDistanceMax);
                                        if (distance < 0)
                                            distance = editDistanceMax + 1;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                        }
                        else if (!suggestionsHashset.Add(suggestion))
                            continue;

                        if ((verbose < 2) && (suggestions.Count > 0) && (distance > suggestions[0].distance))
                            continue;

                        if (distance <= editDistanceMax)
                        {
                            int value2;
                            if (dictionary.TryGetValue(suggestion, out value2))
                            {
                                var suggestedItem = new SuggestedItem()
                                {
                                    word = suggestion,
                                    count = DictionaryGenerator.GetItemsList()[-value2 - 1].appearances,
                                    distance = distance
                                };

                                if (verbose < 2)
                                {
                                    initialEditDistanceMax = distance;
                                }

                                if ((verbose < 2) && (suggestions.Count > 0) && (suggestions[0].distance > distance))
                                    suggestions.Clear();

                                suggestions.Add(suggestedItem);
                            }
                        }
                    }
                }

                if (lengthDiff < editDistanceMax)
                {
                    if (candidate.Length > prefixLength) 
                        candidate = candidate.Substring(0, prefixLength);
                   
                    for (int i = 0; i < candidate.Length; i++)
                    {
                        string delete = candidate.Remove(i, 1);

                        if (deletesHashset.Add(delete)) 
                        { 
                            candidates.Add(delete);
                        }
                    }
                }
            }

            return this.GetSortedList(suggestions);
        }

        private List<string> GetSortedList(List<SuggestedItem> suggestions)
        {
            var suggestedWords = new List<string>();
            if (verbose < 2)
            {
                suggestions.Sort((x, y) => -x.count.CompareTo(y.count));
            }
            else
            {
                suggestions.Sort((x, y) => 2 * x.distance.CompareTo(y.distance) - x.count.CompareTo(y.count));
            }
            foreach (var item in suggestions)
            {
                suggestedWords.Add(item.word);
            }
            if ((verbose == 0) && (suggestions.Count > 1))
            {

                return suggestedWords;
            }
            else
                return suggestedWords;
        }

    }
}