using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Course_Work;

namespace Course_Work
{
    internal class Menu
    {
        public void Start(Course_Work.Index index, ConcurrentDictionary<int, Documnet> baseDocuments)
        {
            Console.WriteLine("1 or 2");
            char keyChar = Console.ReadKey().KeyChar;
            int answer = (int)keyChar - '0';
            Console.WriteLine("\nВведите путь к файлу:");
            string path = Console.ReadLine();
            if (answer == 1)
            {
                SingalThread(index, baseDocuments, path);
            }
            else if (answer == 2)
            {
                Console.Write("Ведите количество потоков:");
                keyChar = Console.ReadKey().KeyChar;
                int countOfThreads = (int)keyChar - '0';
                Console.WriteLine();
                MultyThread(index, baseDocuments, path, countOfThreads);
            }
        }

        public void SingalThread(Course_Work.Index index, ConcurrentDictionary<int, Documnet> baseDocuments, string path)
        {
            string[] files = new string[] { };
            int startNumberFile = 0;
            int endNumberFile = 0;

            string directoryPath = path;

            startNumberFile = Directory.GetFiles(directoryPath).Length / 50 * 18;
            endNumberFile = Directory.GetFiles(directoryPath).Length / 50 * 19;

            for (int i = startNumberFile; i <= endNumberFile; i++)
            {
                int documentId = i;

                Documnet document = new Documnet();

                document.id = documentId;
                document.pathDocument = directoryPath;

                string searchPattern = $"{document.id}_*.txt";

                files = Directory.GetFiles(directoryPath, searchPattern);

                document.text = File.ReadAllText(files[0]);

                string[] words = Regex.Matches(document.text.ToLower(), @"\b\w+\b")
                    .Cast<Match>()
                    .Select(match => match.Value)
                    .ToArray();

                document.TermsSet(words.ToList<string>());

                baseDocuments.TryAdd(document.id, document);
            }

            foreach (var document in baseDocuments.Values)
                foreach (var term in document.GetTerms())
                    index.AddTerm(term, document.id);
        }
        public void MultyThread(Course_Work.Index index, ConcurrentDictionary<int, Documnet> baseDocuments, string path, int countOfThreads)
        {
            CountdownEvent countdownEvent;

            string[] files = new string[] { };
            int startNumberFile = 0;
            int endNumberFile = 0;
            string cuurentDirectoryPath = path;

            string directoryPath = path;

            startNumberFile = Directory.GetFiles(directoryPath).Length / 50 * 18;
            endNumberFile = Directory.GetFiles(directoryPath).Length / 50 * 19;

            countdownEvent = new CountdownEvent(endNumberFile - startNumberFile + 1);

            cuurentDirectoryPath = directoryPath;

            for (int i = startNumberFile; i <= endNumberFile; i++)
            {
                ThreadPool.QueueUserWorkItem(ProcessFile, i);
            }
            countdownEvent.Wait();

            void ProcessFile(object state)
            {
                int documentId = (int)state;
                string directoryPath = cuurentDirectoryPath;

                Documnet document = new Documnet();

                document.id = documentId;
                document.pathDocument = directoryPath;

                string searchPattern = $"{documentId}_*.txt";

                string[] files = Directory.GetFiles(directoryPath, searchPattern);

                document.text = File.ReadAllText(files[0]);

                string[] words = Regex.Matches(document.text.ToLower(), @"\b\w+\b")
                    .Cast<Match>()
                    .Select(match => match.Value)
                    .ToArray();

                document.TermsSet(words.ToList<string>());

                baseDocuments.TryAdd(document.id, document);

                countdownEvent.Signal();
            }

            Parallel.ForEach(baseDocuments.Values, document =>
            {
                foreach (var term in document.GetTerms())
                {
                    index.AddTerm(term, document.id);
                }
            });
        }
    }
}
