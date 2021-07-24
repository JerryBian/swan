using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Helper
{
    public static class StringHelper
    {
        public static bool EqualIgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
