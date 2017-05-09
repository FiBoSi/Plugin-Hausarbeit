using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer1
{
    static class MyContains
    {
        public static bool MyOwnContains(this string str, params string[] p)
        {
            return p.Any(s => str.Contains(s));
        }
    }
}
