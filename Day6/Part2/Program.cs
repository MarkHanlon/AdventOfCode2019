using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day6
{

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Dictionary<string, string> orbitStore = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    string orbits = sr.ReadToEnd();                    

                    var orbitArray = orbits.Split("\r\n");
                    foreach(string orbit in orbitArray)
                    {
                        var orbitParts = orbit.Split(")");                 // "A)B"       
                        orbitStore.Add(orbitParts[1], orbitParts[0]);      // dict["B"] = "A"
                    }

                    // Store all of your ancestors in a Dictionary
                    Dictionary<string, int> myAncestors = new Dictionary<string, int>(); // String=Orbit object, int=steps away
                    string pos = "YOU";
                    int level = 0;
                    while (orbitStore.ContainsKey(pos))
                    {
                        myAncestors.Add(pos = orbitStore[pos], level++);                        
                    }

                    // Now do the same from "SAN", looking for a common ancestor
                    pos = "SAN";
                    level = 0;
                    while (orbitStore.ContainsKey(pos))
                    {
                        pos = orbitStore[pos];
                        if (myAncestors.ContainsKey(pos))
                        {
                            level += myAncestors[pos];
                            break;    
                        }                        
                        level++;
                    }

                    // Done
                    timer.Stop();
                    Console.WriteLine($"Answer {level}, in {timer.ElapsedMilliseconds}ms");
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
