using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransAide.Model
{
    public class TranslationUnitList
    {
        [JsonPropertyName("translation_units")]
        public List<TranslationUnit> TranslationUnits { get; set; }
        public TranslationUnitList()
        {
            TranslationUnits = new List<TranslationUnit>();
        }
        public void AddTranslationUnit(TranslationUnit translationUnit)
        {
            TranslationUnits.Add(translationUnit);
        }
        public void RemoveTranslationUnit(TranslationUnit translationUnit)
        {
            TranslationUnits.Remove(translationUnit);
        }
        public void ClearTranslationUnits()
        {
            TranslationUnits.Clear();
        }
        public int Count()
        {
            return TranslationUnits.Count;
        }
               
        
    }
}
