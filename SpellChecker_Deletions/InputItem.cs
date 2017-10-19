using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellCheck_Deletions
{
    internal class InputItem
    {
        internal long appearances = 0;
        internal List<Int32> suggestions = new List<Int32>(2);
    }
}
