using SpellCheck_PN.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellCheck_PN
{
    public class SpellChecker
    {
        const string BG_DICTIONARY_PATH = @"/Dictionaries/bg_frequency_dictionary.txt";

        private Dictionary<string, int> wordsDictionary;

        public SpellChecker()
        {
            this.InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            if (this.wordsDictionary == null || this.wordsDictionary.Count == 0)
            {
                var bgDictionaryFilePath = Directory.GetCurrentDirectory() + BG_DICTIONARY_PATH;
                this.wordsDictionary = DictionaryGenerator.LoadDictionary(bgDictionaryFilePath, "bg", 0, 1);
            }
        }

        public List<string> Correct(string word, string language)
        {
            var candidates = new List<string>();

            if (string.IsNullOrEmpty(word))
                return candidates;

            word = word.ToLower();

            // known()
            if (this.wordsDictionary.ContainsKey(word))
            {
                candidates.Add(word);
                return candidates;
            }

            List<String> edits = Edits(word, language);

            foreach (string wordVariation in edits)
            {
                if (this.wordsDictionary.ContainsKey(wordVariation) && !candidates.Contains(wordVariation))
                    candidates.Add(wordVariation);
            }

            if (candidates.Count > 0)
                return candidates;

            foreach (string item in edits)
            {
                var knownEdits = Edits(item, language);
                foreach (string wordVariation in knownEdits)
                {
                    if (this.wordsDictionary.ContainsKey(wordVariation) && !candidates.Contains(wordVariation))
                        candidates.Add(wordVariation);
                }
            }

            return (candidates.Count > 0) ? candidates : new List<string>();
        }

        private List<string> Edits(string word, string language)
        {
            var splits = new List<Tuple<string, string>>();
            var transposes = new List<string>();
            var deletes = new List<string>();
            var replaces = new List<string>();
            var inserts = new List<string>();

            // Splits
            for (int i = 0; i < word.Length; i++)
            {
                var tuple = new Tuple<string, string>(word.Substring(0, i), word.Substring(i));
                splits.Add(tuple);
            }

            // Deletes
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (!string.IsNullOrEmpty(b))
                {
                    deletes.Add(a + b.Substring(1));
                }
            }

            // Transposes
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (b.Length > 1)
                {
                    transposes.Add(a + b[1] + b[0] + b.Substring(2));
                }
            }

            // Replaces
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (!string.IsNullOrEmpty(b))
                {
                    if (language == "bg")
                    {
                        ReplaceBulgarianLetters(replaces, a, b);
                    }
                    else
                    {
                        ReplaceEnglishLetters(replaces, a, b);
                    }                    
                }
            }

            // Inserts
            for (int i = 0; i < splits.Count; i++)
            {
                string a = splits[i].Item1;
                string b = splits[i].Item2;
                if (language == "bg")
                {
                    InsertBulgarianLetters(inserts, a, b);
                }
                else
                {
                    InsertEnghlishLetters(inserts, a, b);
                }
            }

            return deletes.Union(transposes).Union(replaces).Union(inserts).ToList();
        }

        private static void ReplaceEnglishLetters(List<string> replaces, string a, string b)
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                replaces.Add(a + c + b.Substring(1));
            }
        }
        private static void ReplaceBulgarianLetters(List<string> replaces, string a, string b)
        {
            for (char c = 'а'; c <= 'я'; c++)
            {
                replaces.Add(a + c + b.Substring(1));
            }
        }

        private static void InsertEnghlishLetters(List<string> inserts, string a, string b)
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                inserts.Add(a + c + b);
            }
        }

        private static void InsertBulgarianLetters(List<string> inserts, string a, string b)
        {
            for (char c = 'а'; c <= 'я'; c++)
            {
                inserts.Add(a + c + b);
            }
        }
    }
}