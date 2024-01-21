using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Work
{
    internal class Documnet
    {
        public int id { get; set; }
        public string text { get; set; }

        private List<string> terms = new List<string>();

        public string pathDocument { get; set; }

    public void TermsSet(List<string> terms) { this.terms = terms; }

        public List<string> GetTerms() { return terms; }
    }
}
