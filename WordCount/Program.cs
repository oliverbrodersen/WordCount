using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WordCount
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setting variables
            var watchSearch = new System.Diagnostics.Stopwatch();
            var watchDisplay = new System.Diagnostics.Stopwatch();
            List<string> textSplit = new();
            Dictionary<string, int> dic = new();

            Console.WriteLine("File path:");
            string path = Console.ReadLine();

            //Start indexing
            watchSearch.Start();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    textSplit.AddRange(line.Split(new[] { Environment.NewLine, " ", ",", ".", "\"" }, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            //Counting
            foreach(string s in textSplit)
            {
                if (!dic.ContainsKey(s))
                    dic.Add(s, 1);
                else
                    dic[s]++;
            }
            watchSearch.Stop();

            //Start Displaying
            watchDisplay.Start();
            List<string> outList = dic.Select((x) => $"{x}").ToList();
            File.WriteAllLinesAsync("WriteLines.txt", outList);
            watchDisplay.Stop();

            Console.WriteLine($"Hyppiste ord: {dic.OrderByDescending(x => x.Value).First()}");
            Console.WriteLine($"Total time: {watchDisplay.ElapsedMilliseconds + watchSearch.ElapsedMilliseconds}ms, Search time: {watchSearch.ElapsedMilliseconds}ms, Display time: {watchDisplay.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }
    }
}
