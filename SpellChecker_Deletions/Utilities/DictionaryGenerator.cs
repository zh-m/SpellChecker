using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpellCheck_Deletions.Utilities
{
    internal class DictionaryGenerator
    {
        public static int maxlength = 0;
        public static int prefixLength = 7;
        public static int editDistanceMax = 2;

        private static List<InputItem> itemsList = new List<InputItem>();
        private static List<string> uniqueWordList = new List<string>();

        //load a frequency dictionary
        public static Dictionary<string, int> LoadDictionary(string freaquencyDictionaryText, string language, int termIndex, int countIndex)
        {
            var dictionary = new Dictionary<string, int>();
            if (File.Exists(freaquencyDictionaryText))
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(freaquencyDictionaryText)))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var lineParts = line.Split(null);
                        if (lineParts.Length >= 2)
                        {
                            string word = lineParts[termIndex];

                            Int64 count;
                            if (Int64.TryParse(lineParts[countIndex], out count))
                            {
                                CreateDictionaryEntry(dictionary, word, language, Math.Min(Int64.MaxValue, count));
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        public static bool CreateDictionaryEntry(Dictionary<string, int> dictionary, string key, string language, Int64 count)
        {
            int countTreshold = 1;
            long countPrevious = 0;
            bool result = false;
            InputItem inputItem = null;
            int value;
            
            if (dictionary.TryGetValue(key, out value))
            {
                if (value >= 0)
                {
                    var tmp = value;
                    inputItem = new InputItem();
                    inputItem.suggestions.Add(tmp);
                    DictionaryGenerator.itemsList.Add(inputItem);
                    dictionary[key] = -DictionaryGenerator.itemsList.Count;
                }
                else
                {
                    inputItem = DictionaryGenerator.itemsList[-value - 1];
                }

                countPrevious = inputItem.appearances;
                inputItem.appearances = Math.Min(long.MaxValue, inputItem.appearances + count);
            }
            else
            {
                //new word
                inputItem = new InputItem()
                {
                    appearances = count
                };
                DictionaryGenerator.itemsList.Add(inputItem);
                dictionary[key] = -DictionaryGenerator.itemsList.Count;

                if (key.Length > maxlength) 
                    maxlength = key.Length;
            }

            if ((inputItem.appearances >= countTreshold) && (countPrevious < countTreshold))
            {
                uniqueWordList.Add(key);
                int keyint = (int)(uniqueWordList.Count - 1);

                foreach (string delete in EditsPrefix(key))
                {
                    InputItem deletionItem;
                    int deletionValue;
                    if (dictionary.TryGetValue(delete, out deletionValue))
                    {
                        if (deletionValue >= 0)
                        {
                            deletionItem = new InputItem();
                            {
                                deletionItem.suggestions.Add(deletionValue);
                                DictionaryGenerator.itemsList.Add(deletionItem);
                                dictionary[delete] = -DictionaryGenerator.itemsList.Count;
                            }
                            if (!deletionItem.suggestions.Contains(keyint)) 
                                deletionItem.suggestions.Add(keyint);
                        }
                        else
                        {
                            deletionItem = DictionaryGenerator.itemsList[-deletionValue - 1];
                            if (!deletionItem.suggestions.Contains(keyint)) 
                                deletionItem.suggestions.Add(keyint);
                        }
                    }
                    else
                    {
                        dictionary.Add(delete, keyint);
                    }

                }
            }
            return result;
        }

        private static HashSet<string> Edits(string word, int editDistance, HashSet<string> deletes)
        {
            editDistance++;
            if (word.Length > 1)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    string delete = word.Remove(i, 1);
                    if (deletes.Add(delete))
                    {
                        if (editDistance < editDistanceMax)
                            Edits(delete, editDistance, deletes);
                    }
                }
            }
            return deletes;
        }

        public static HashSet<string> EditsPrefix(string key)
        {
            HashSet<string> hashSet = new HashSet<string>();
            if (key.Length <= editDistanceMax)
                hashSet.Add(""); 

            return Edits(key.Length <= prefixLength ? key : key.Substring(0, prefixLength), 0, hashSet);
        }

        public static List<string> GetUniqueWordList()
        {
            return uniqueWordList;
        }

        public static List<InputItem> GetItemsList()
        {
            return itemsList;
        }
    }
}
