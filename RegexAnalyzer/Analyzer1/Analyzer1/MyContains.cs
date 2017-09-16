using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer1
{
    // prüft ob im übergebenen string str strings s aus dem string[] p zu finden sind
    static class MyContains
    {
        public static bool MyOwnContains(this string str, params string[] p)
        {
            return p.Any(s => str.Contains(s));
        }
    }
    // sollte prüfen ob die strings damit beginnen
}
