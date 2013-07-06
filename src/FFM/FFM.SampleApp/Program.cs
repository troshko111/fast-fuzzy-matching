using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FFM.SampleApp
{
    internal class Program
    {
        private const string Usage =
            "SampleApp.exe <pathToFileWithSomeWords> <desiredWordsCount> <maxWordDistanceToMatch> [<randomQueriesToRun>].\n"
            + "(If you do not have file with enough number of words, download one from "
            + "http://dumps.wikimedia.org/enwiki/latest/).";

        private const int NotificationStepForTreeBuilding = 10000;
        private const int NotificationStepForRandomEuqries = 100;

        private static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine(Usage);
                return;
            }

            var desiredWords = 0;
            var maxDistance = 0;
            var randomQueriesToRun = 0;

            if (args.Length >= 3)
            {
                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("{0} does not exist.", args[0]);
                    return;
                }
                if (!int.TryParse(args[1], out desiredWords))
                {
                    Console.WriteLine("{0} is not valid desired words count (integer).", args[1]);
                    return;
                }
                if (!int.TryParse(args[2], out maxDistance))
                {
                    Console.WriteLine("{0} is not valid max distance to match (integer).", args[2]);
                    return;
                }
            }
            if (args.Length == 4)
            {
                if (!int.TryParse(args[3], out randomQueriesToRun))
                {
                    Console.WriteLine("{0} is not valid random queries to run (integer).", args[3]);
                    return;
                }

                RunRandomTestsing(args[0], desiredWords, maxDistance, randomQueriesToRun);
            }
            else
            {
                RunInInteractiveMode(args[0], desiredWords, maxDistance);
            }
        }

        private static void RunRandomTestsing(string filePath, int desiredWords, int maxDistance, int randomQueriesToRun)
        {
            Console.WriteLine("Random testing.\nFile: {0}, words: {1}, maxDistance: {2}, queries: {3}.",
                              filePath,
                              desiredWords,
                              maxDistance,
                              randomQueriesToRun);

            HashSet<string> words = null;
            var stepName = "LoadWordsFromFile";
            MeasureMemory(() => MeasureExecutionTime(() => words = LoadWordsFrom(filePath, desiredWords), stepName), stepName);
            Console.WriteLine("Total words processed: {0}.", words.Count);

            BKTree<string> tree = null;
            stepName = "Build BK-tree";
            MeasureMemory(() => MeasureExecutionTime(() => tree = BuildBKTree(words), stepName), stepName);

            stepName = "Random testing";
            var wordsList = words.ToList();
            MeasureMemory(() => MeasureExecutionTime(() => RunQueries(tree, wordsList, maxDistance, randomQueriesToRun), stepName), stepName);
        }

        private static void RunInInteractiveMode(string filePath, int desiredWords, int maxDistance)
        {
            Console.WriteLine("Interactive mode.\nFile: {0}, words: {1}, maxDistance: {2}.",
                              filePath,
                              desiredWords,
                              maxDistance);

            HashSet<string> words = null;
            var stepName = "LoadWordsFromFile";
            MeasureMemory(() => MeasureExecutionTime(() => words = LoadWordsFrom(filePath, desiredWords), stepName), stepName);
            Console.WriteLine("Total words processed: {0}.", words.Count);

            BKTree<string> tree = null;
            stepName = "Build BK-tree";
            MeasureMemory(() => MeasureExecutionTime(() => tree = BuildBKTree(words), stepName), stepName);

            stepName = "Interactive querying";
            while (true)
            {
                Console.Write("Query: ");
                var query = Console.ReadLine();
                if (!string.IsNullOrEmpty(query))
                    query = query.ToLowerInvariant();

                List<string> matches = null;
                MeasureMemory(() => MeasureExecutionTime(() => matches = FindMatches(tree, query, maxDistance), stepName), stepName);
                Console.WriteLine("Found {0} matches.", matches.Count);
                foreach (var match in matches)
                    Console.WriteLine(match);
            }
        }

        private static void MeasureExecutionTime(Action action, string actionName)
        {
            var watch = Stopwatch.StartNew();
            action();
            watch.Stop();

            var time = watch.Elapsed.TotalSeconds >= 1
                           ? string.Format("{0}s", watch.Elapsed.TotalSeconds)
                           : string.Format("{0}ms", watch.Elapsed.TotalMilliseconds);

            Console.WriteLine("{0} took {1}.", actionName, time);
        }

        private static void MeasureMemory(Action action, string actionName)
        {
            var bytesBefore = GC.GetTotalMemory(true);
            action();
            var bytesAfter = GC.GetTotalMemory(true);

            var kb = (bytesAfter - bytesBefore)/1024;
            var memory = kb >= 1024 ? string.Format("{0}MB", kb/1024) : string.Format("{0}KB", kb);
            Console.WriteLine("{0} took {1}.", actionName, memory);
        }

        private static HashSet<string> LoadWordsFrom(string filePath, int desiredWords)
        {
            var lines = File.ReadLines(filePath);
            var words = new HashSet<string>();

            foreach (var line in lines)
            {
                if (words.Count > desiredWords)
                    break;

                var splitted = Regex.Split(line, @"\W+", RegexOptions.Compiled).Where(s => !string.IsNullOrEmpty(s));
                foreach (var w in splitted)
                    words.Add(w.ToLowerInvariant());
            }

            return new HashSet<string>(words.Shuffle());
        }

        private static BKTree<string> BuildBKTree(IEnumerable<string> words)
        {
            var tree = new BKTree<string>(new DamerauLevenshteinStringDistanceMeasurer());

            var insertionProgress = 0;
            foreach (var word in words)
            {
                if (insertionProgress++ % NotificationStepForTreeBuilding == 0)
                    Console.WriteLine("Inserted {0} words into BK-tree.", insertionProgress - 1);
                tree.Add(new BKTreeNode<string>(word));
            }

            return tree;
        }

        private static List<string> FindMatches(BKTree<string> tree, string query, int maxDistance)
        {
            return tree.Matches(query, maxDistance);
        }

        private static void RunQueries(BKTree<string> tree, IList<string> words, int maxDistance, int totalQueries)
        {
            var history = new List<TimeSpan>(totalQueries);
            var random = new Random();
            var watch = new Stopwatch();
            for (var i = 0; i < totalQueries; i++)
            {
                if (i%NotificationStepForRandomEuqries == 0)
                    Console.WriteLine("{0} queries executed.", i);

                var word = words[random.Next(0, words.Count)];

                watch.Start();
                var matches = FindMatches(tree, word, maxDistance);
                watch.Stop();

                history.Add(watch.Elapsed);
                watch.Reset();
            }

            Console.WriteLine("Finished running random queries.\nAverage query time {0}ms.",
                              history.Average(x => x.TotalMilliseconds));
        }
    }
}