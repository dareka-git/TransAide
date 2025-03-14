using System;
using System.Collections.Generic;
using System.Linq;

namespace TransAide.Model
{
    public class GlossaryTermCollection
    {
        private HashSet<GlossaryTerm> glossaryTerms;

        public GlossaryTermCollection()
        {
            glossaryTerms = new HashSet<GlossaryTerm>(new GlossaryTermComparer());
        }

        public void AddGlossaryTerm(GlossaryTerm term)
        {
            glossaryTerms.Add(term);
        }

        public void AddGlossaryTerms(List<GlossaryTerm> terms)
        {
            foreach (var term in terms)
            {
                glossaryTerms.Add(term);
            }
        }

        public List<GlossaryTerm> GetSortedGlossaryTerms()
        {
            return glossaryTerms.OrderBy(term => term.SourceTerm).ToList();
        }

        public List<GlossaryTerm> GetSortedRequiredGlossaryTerms()
        {
            return glossaryTerms.Where(term => !term.IsForbidden).OrderBy(term => term.SourceTerm).ToList();
        }

        public List<GlossaryTerm> GetSortedForbiddenGlossaryTerms()
        {
            return glossaryTerms.Where(term => term.IsForbidden).OrderBy(term => term.SourceTerm).ToList();
        }

        public string GetGlossaryTermsAsString(bool isForbidden)
        {
            var filteredTerms = glossaryTerms.Where(term => term.IsForbidden == isForbidden)
                                             .OrderBy(term => term.SourceTerm)
                                             .Select(term => term.ToString())
                                             .ToList()
                                             .Aggregate((a, b) => a + Environment.NewLine + b);
            return filteredTerms;
        }

        private class GlossaryTermComparer : IEqualityComparer<GlossaryTerm>
        {
            public bool Equals(GlossaryTerm x, GlossaryTerm y)
            {
                if (x == null || y == null)
                    return false;

                return x.SourceTerm == y.SourceTerm && x.TargetTerms.SequenceEqual(y.TargetTerms) && x.IsForbidden == y.IsForbidden;
            }

            public int GetHashCode(GlossaryTerm obj)
            {
                if (obj == null)
                    return 0;

                int hashSourceTerm = obj.SourceTerm == null ? 0 : obj.SourceTerm.GetHashCode();
                int hashTargetTerms = obj.TargetTerms == null ? 0 : obj.TargetTerms.Aggregate(0, (current, term) => current ^ term.GetHashCode());
                int hashIsForbidden = obj.IsForbidden.GetHashCode();

                return hashSourceTerm ^ hashTargetTerms ^ hashIsForbidden;
            }
        }
    }
}
