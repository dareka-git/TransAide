using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Model
{
    public class DocumentFile
    {
        public string Name { get; set; } // .Name
        public string LocalFilePath { get; set; } //.LocalFilePath
        public Guid Id { get; set; } // .Id
        public int TotalSegmentPairsCount { get; set; } // 
    }
}
