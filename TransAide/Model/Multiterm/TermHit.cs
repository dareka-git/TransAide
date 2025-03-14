using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Model.Multiterm
{
    public class TermHit
    {
        public TermHit()
        {
            TermEntries = new List<TermEntry>();
        }

        public string Text { get; set; }

        public int SearchScore { get; set; }

        public string Termbase { get; set; }

        public string Language { get; set; }

        public string ParentEntryId { get; set; }

        public List<TermEntry> TermEntries { get; set; }
    }
}
