using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Course_Work;

class Program
{
    static void Main()
    {
        ConcurrentDictionary<int, Documnet> baseDocuments = new ConcurrentDictionary<int, Documnet>();
        Course_Work.Index index = new();

        Menu menu = new Menu();
        menu.Start(index, baseDocuments);

        string searchWord = "movie";

        if (index.ContainsTerm(searchWord))
        {
            foreach (var documentId in index.GetDocumentIdsForTerm(searchWord))
            {
                Console.WriteLine(documentId);
            }
        }
    }
}