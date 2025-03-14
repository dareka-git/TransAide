using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Extension
{
    public static class StringExtension
    {
        public static bool Contains(this String str, String substring, StringComparison comp)
        {
            if (substring == null)
            {
                throw new ArgumentNullException("substring", "substring cannot be null.");
            }
            else if (!System.Enum.IsDefined(typeof(StringComparison), comp))
            {
                throw new ArgumentException("comp is not a member of StringComparison", "comp");
            }
           
            return str.IndexOf(substring, comp) >= 0;
        }


        public static bool IsForbidden(this String str, List<string> additionalForbiddenStatuses)
        {
            List<string> forbiddenStatuses = new List<string>
            {
                "forbidden", "obsolete", "deprecated", "prohibited", "banned", "not allowed", "not permitted", "unacceptable"
            };

            if (additionalForbiddenStatuses == null)
            {
                throw new ArgumentNullException("additionalForbiddenStatuses", "additional status list cannot be null; otherwise use other method overload.");
            }
            else
            {
                forbiddenStatuses.AddRange(additionalForbiddenStatuses);
            }

            foreach (string forbiddenStatus in forbiddenStatuses)
            {
                if (str.IndexOf(forbiddenStatus, StringComparison.OrdinalIgnoreCase) >= 0) { return true; }
            }

            return false;
        }


        public static bool IsForbidden(this String str)
        {
            List<string> forbiddenStatuses = new List<string>
            {
                "forbidden", "obsolete", "deprecated", "prohibited", "banned", "not allowed", "not permitted", "unacceptable"
            };

            foreach (string forbiddenStatus in forbiddenStatuses)
            {
                if (str.IndexOf(forbiddenStatus, StringComparison.OrdinalIgnoreCase) >= 0) { return true; }
            }

            return false;
        }
    }
}
