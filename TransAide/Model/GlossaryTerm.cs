using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Model
{
    public class GlossaryTerm
    {
        public string SourceTerm { get; set; }
        public List<string> TargetTerms { get; set; }
        public bool IsForbidden { get; set; }

        public GlossaryTerm() { }

        public GlossaryTerm(string sourceTerm, List<string> targetTerms, bool isForbidden)
        {
            SourceTerm = sourceTerm;
            TargetTerms = targetTerms;
            IsForbidden = isForbidden;
        }

        public GlossaryTerm(string sourceTerm, List<string> targetTerms)
        {
            SourceTerm = sourceTerm;
            TargetTerms = targetTerms;
            IsForbidden = false;
        }

        public GlossaryTerm(string sourceTerm, string targetTerm, bool isForbidden)
        {
            SourceTerm = sourceTerm;
            TargetTerms = new List<string> { targetTerm };
            IsForbidden = isForbidden;
        }

        public GlossaryTerm(string sourceTerm, string targetTerm)
        {
            SourceTerm = sourceTerm;
            TargetTerms = new List<string> { targetTerm };
            IsForbidden = false;
        }

        public override string ToString()
        {
            return SourceTerm + "\t" + TargetTerms.OrderBy(x => x).Aggregate((a, b) => a + ", " + b);
        }
    }
}
