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

            // Solution: Only need to worry about digits from position offset 5970417 (which is 93% of the way through the list)
            // Any more than half way through the list and the digits are just the sum of the last set of digits, and each digit
            // along is the previous digit's sum take away one digit at your position.
            // You only need to calculate 8 digits of answer too, I believe. 

            // SO it's:
            // - Sum all digits from posn 5970417 to the end, store this and the next phase digit is this sum % 10
            // - For the next 7 digits, subtract from the sum the digit at position 5970418, 5970419, etc.
            // - Repeat this iteration 100 times 

            using (StreamReader sr = new StreamReader("input.txt"))
            {
                string inputStr = sr.ReadToEnd();                                    
                var input = Enumerable.Repeat(inputStr, 10000).SelectMany(x => x).Select(x => Int32.Parse(x.ToString())).ToArray();
               
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
            int sum = 0;
            int pos = 0;
            foreach(int i in input)
            {
                if (pattern[pos] != 0)
                {
                    sum += (i * pattern[pos++]);
                }
                else pos++;
            }
            return (Math.Abs(sum) % 10);
            //return Math.Abs(input.Select((x, i) => x * pattern[i]).Sum()) % 10;
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
