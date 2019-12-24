using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day3
{

        public class StringLen
        {
            int[] stringLen = new int[2];

            public StringLen(int stringId, int len)
            {
                SetStringLen(stringId, len);
            }

            public void SetStringLen(int stringId, int len)
            {
                // Don't overwrite with self-crossing strings
                if (stringLen[stringId] == 0)
                    stringLen[stringId] = len;
            }

            public bool IsIntersecting()
            {
                if (stringLen[0] != 0 && stringLen[1] != 0)
                    return true;
                else
                    return false;
            }

            public int IntersectingLength()
            {
                return stringLen[0] + stringLen[1];
            }
        }

    class Program
    {

        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            try
            {
                Dictionary<(int x, int y), StringLen> dict = new Dictionary<(int x, int y), StringLen>();
                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    string wireStr;
                    int stringNum = 0;
                    while((wireStr = sr.ReadLine()) != null)
                    {                    
                        int x = 0, y = 0;
                        int totalLength = 1;
                        string[] wireSteps = wireStr.Split(',');
                        foreach (string step in wireSteps)
                        {
                            int len = Int16.Parse(step.Substring(1));
                            switch(step[0])
                            {
                                case 'U':      
                                    for (int newY = y + 1; newY <= y + len; newY++)
                                        if (dict.ContainsKey((x, newY)))
                                            dict[(x, newY)].SetStringLen(stringNum, totalLength++);
                                        else
                                            dict[(x, newY)] = new StringLen(stringNum, totalLength++);
                                    y += len;                                                                                                                                                     
                                    break;
                                case 'D':                                    
                                    for (int newY = y - 1; newY >= y - len; newY--)
                                        if (dict.ContainsKey((x, newY)))
                                            dict[(x, newY)].SetStringLen(stringNum, totalLength++);
                                        else
                                            dict[(x, newY)] = new StringLen(stringNum, totalLength++);
                                    y -= len;
                                    break;
                                case 'R':                                    
                                    for (int newX = x + 1; newX <= x + len; newX++)
                                        if (dict.ContainsKey((newX, y)))
                                            dict[(newX, y)].SetStringLen(stringNum, totalLength++);
                                        else
                                            dict[(newX, y)] = new StringLen(stringNum, totalLength++);
                                    x += len;
                                    break;
                                case 'L':    
                                    for (int newX = x - 1; newX >= x - len; newX--)
                                        if (dict.ContainsKey((newX, y)))
                                            dict[(newX, y)].SetStringLen(stringNum, totalLength++);
                                        else
                                            dict[(newX, y)] = new StringLen(stringNum, totalLength++);
                                    x -= len;
                                    break;
                            }
                        } 

                        stringNum++;                       
                    }
                    
                    // Find all the intersections
                    int min = dict.Values.Where(x => x.IsIntersecting()).Min(l => l.IntersectingLength());

                    timer.Stop();
                    Console.WriteLine($"Answer is {min} in {timer.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                return;
            }
        }

    }
}
