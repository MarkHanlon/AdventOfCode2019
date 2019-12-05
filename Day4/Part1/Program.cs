using System;
using System.Diagnostics;

namespace Part1
{
    class Program
    {
        static void Main(string[] args)
        {
            // 136760-595730
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int count = 0;
            for (int i = 136760; i <= 595730; i++)
            {
                string s = i.ToString();
                // Does it have 2 adjacent digits the same?
                if (s[0] == s[1] ||
                    s[1] == s[2] ||
                    s[2] == s[3] ||
                    s[3] == s[4] ||
                    s[4] == s[5])
                {
                    // Do the digits never decrease, going from left to right
                    if (s[0] <= s[1] &&
                        s[1] <= s[2] &&
                        s[2] <= s[3] &&
                        s[3] <= s[4] &&
                        s[4] <= s[5])
                    {
                        count++;
                    }
                }
            }

            timer.Stop();
            Console.WriteLine($"Answer is {count} in {timer.ElapsedMilliseconds}ms");
        }
    }
}
