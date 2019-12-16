using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {            
            var timer = new Stopwatch();
            timer.Start();

            using (StreamReader sr = new StreamReader("input.txt"))
            {
                string inputStr = sr.ReadToEnd();                                    
                var input = inputStr.ToCharArray().Select(x => Int32.Parse(x.ToString())).ToArray();
               
                int[] output = new int[input.Length];                
                for (int rounds = 0 ; rounds < 100; rounds++)
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        int[] pattern = CreatePattern(i+1, input.Length);
                        output[i] = ApplyPattern(input, pattern);
                    }
                    input = output;
                }

                timer.Stop();
                Console.WriteLine($"Answer is {string.Join("", output.Take(8))}, in {timer.ElapsedMilliseconds}ms");
            }
        }

        private static int ApplyPattern(int[] input, int[] pattern)
        {
            return Math.Abs(input.Select((x, i) => x * pattern[i]).Sum()) % 10;
        }

        private static int[] CreatePattern(int v, int len)
        {
            // Pattern is 0 : 1 : 0 : -1
            return Enumerable.Repeat(
                        Enumerable.Repeat(0, v)
                        .Concat(Enumerable.Repeat(1, v))
                        .Concat(Enumerable.Repeat(0, v))
                        .Concat(Enumerable.Repeat(-1, v)),  len / (v * 4) + 1 )
                    .SelectMany(x => x).Take(len+1).Skip(1).ToArray();
        }
    }
}
