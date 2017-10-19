using System;

namespace SpellCheck_Deletions
{
    public class SuggestedItem
    {
        public string word = "";
        public int distance = 0;
        public Int64 count = 0;

        public override bool Equals(object obj)
        {
            return Equals(word, ((SuggestedItem)obj).word);
        }

        public override int GetHashCode()
        {
            return word.GetHashCode();
        }
    }
}
