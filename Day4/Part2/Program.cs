using System;
using System.Diagnostics;
using System.Linq;

namespace Part2
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
                char[] s = i.ToString().ToArray();
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
                        // Now ensure that there is a group of matching
                        // characters with exactly 2 in the group 
                        if (s.GroupBy(c => c).Any(grp => grp.Count() == 2))
                            count++;
                    }
                }
            }

            timer.Stop();
            Console.WriteLine($"Answer is {count} in {timer.ElapsedMilliseconds}ms");
        }
    }
}
