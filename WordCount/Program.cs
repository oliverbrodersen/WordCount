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
            Dictionary<string, int> dic = new();

            int numberOfGroups = Environment.ProcessorCount;
            int finished = 0;

            watchSearch.Start();
            
            string text = File.ReadAllText("../../../file.txt").ToLower();
            string[] textSplit = text.Split(new[] { Environment.NewLine, " ", ",",".","--", "\"" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string s in textSplit)
            {
                if (dic.ContainsKey(s))
                    dic[s]++;
                else
                    dic.Add(s, 1);
            }
            int uord = dic.Count;

            watchSearch.Stop();
            watchDisplay.Start();

            Action onCompleted = () =>
            {
                if (finished == numberOfGroups - 1)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Unikke ord: {uord}");
                    watchDisplay.Stop();
                    Console.WriteLine($"Hyppiste ord: {dic.OrderByDescending(x => x.Value).First().ToString()}");
                    Console.WriteLine($"Total time: {watchDisplay.ElapsedMilliseconds + watchSearch.ElapsedMilliseconds}ms, Search time: {watchSearch.ElapsedMilliseconds}ms, Display time: {watchDisplay.ElapsedMilliseconds}ms");
                    Console.ReadLine();
                }
                else
                    finished++;
            };

            //var result = dic.GroupBy(x => counter++ % numberOfGroups);
            var dicList = dic
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / ((uord / numberOfGroups)))
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();

            for (int i = 0; i < dicList.Count -1 ; i++)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        print(dicList[i]);
                    }
                    finally
                    {
                        onCompleted();
                    }
                });
                thread.Start();
            }

            //Search time: 84ms, Display time: 4731ms <= single thread
            //Search time: 72ms, Display time: 1026ms
        }

        private static void print(List<KeyValuePair<string, int>> list)
        {
            int r = 10;
            int space = 32;
            for (int i = 0; i < list.Count; i+= r)
            {
                string s = "";
                for (int j = 0; j < r; j++)
                {
                    try
                    {
                        string st = $"[{list[i + j].Value}] {list[i + j].Key}";
                        s += st;
                        //Spaceing
                        for (int k = 0; k < space - st.Length; k++)
                        {
                            s += " ";
                        }
                    }
                    catch
                    {
                        //stop if no more elements in list
                    }
                }
                Console.WriteLine(s);
            }
        }
    }

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}
