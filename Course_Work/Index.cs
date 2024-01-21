using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Course_Work
{
    internal class Index
    {
        ConcurrentDictionary<string, ConcurrentDictionary<int, int>> invertIndex = new ConcurrentDictionary<string, ConcurrentDictionary<int, int>>();

        public void AddTerm(string term, int documentId)
        {
            invertIndex.GetOrAdd(term, _ => new ConcurrentDictionary<int, int>())
            .AddOrUpdate(documentId, _ => 1, (_, count) => count + 1);
        }

        public void Print()
        {
            foreach (var index in invertIndex)
            {
                Console.Write(index.Key + ", DID's:");
                foreach (var element in index.Value)
                {
                    Console.Write(element + ",");
                }
                Console.WriteLine();
            }
        }

        public IEnumerable<int> GetDocumentIds()
        {
            return invertIndex.SelectMany(pair => pair.Value.Keys).Distinct();
        }

        public bool ContainsTerm(string term)
        {
            return invertIndex.ContainsKey(term);
        }

        public IEnumerable<int> GetDocumentIdsForTerm(string term)
        {
            return invertIndex.TryGetValue(term, out var documents) ? documents.Keys : Enumerable.Empty<int>();
        }


    }
}
