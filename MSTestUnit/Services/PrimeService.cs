using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTestUnit.Services
{
    public class PrimeService
    {
        public int CheckInt(string[] strs)
        {
            if (strs.Any(s=>s.Contains("ab")))
                return 1;
            return 0;
        }
    }
}
