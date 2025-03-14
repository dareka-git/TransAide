using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Model.Multiterm
{
    public class TermEntry
    {
        public string Term { get; set; }

        public string Language { get; set; }

        public List<EntryField> IndexFields { get; set; }

        public List<EntryField> TermFields { get; set; }

        public bool ReadOnly { get; set; }
    }
}
