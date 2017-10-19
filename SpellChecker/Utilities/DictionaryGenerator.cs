using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpellCheck_PN.Utilities
{
    internal class DictionaryGenerator
    {
        public static Dictionary<string, int> LoadDictionary(string freaquencyDictionaryText, string language, int termIndex, int countIndex)
        {
            var words = new Dictionary<string, int>();
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

                            int count;
                            if (int.TryParse(lineParts[countIndex], out count))
                            {
                                words.Add(word, count);
                            }
                        }
                    }
                }
            }
            return words;
        }
    }
}
