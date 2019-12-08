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

                    // Now walk every path counting the number of links
                    int links = 0;
                    foreach(string key in orbitStore.Keys)    
                    {                        
                        string leaf = key;
                        while (orbitStore.ContainsKey(leaf))
                        {
                            leaf = orbitStore[leaf];                            
                            links++;    
                        }
                        
                    }

                    // Done
                    timer.Stop();
                    Console.WriteLine($"Answer {links}, in {timer.ElapsedMilliseconds}ms");
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
