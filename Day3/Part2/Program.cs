using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{

    class Program
    {

        // TO SOLVE PART 2-------------------------------------
        // For each line segment that is intersected, store the 
        // position along the line where the intersection is.
        // (Mark the line as an intersection, and also save the 
        //  intersection in its own dictionary)
        // Then walk each wire in segment order, adding up the 
        // length/distance as you go until you hit an intersection
        // segment.
        // On the intersection dictionary, record the distance travelled,
        // for each wire. Find the intersection with the lowest summed
        // distance for each wire.
        static void Main(string[] args)
        {

            try
            {
                var vDict = new Dictionary<Tuple<int,int>, int>[2]{new Dictionary<Tuple<int,int>, int>(), 
                                                                   new Dictionary<Tuple<int,int>, int>()};
                var hDict = new Dictionary<Tuple<int,int>, int>[2]{new Dictionary<Tuple<int,int>, int>(), 
                                                                   new Dictionary<Tuple<int,int>, int>()};
                int stringNum = 0;

                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    string wireStr;
                    while((wireStr = sr.ReadLine()) != null)
                    {                    
                        int x = 0, y = 0;
                        string[] wireSteps = wireStr.Split(',');
                        foreach (string step in wireSteps)
                        {
                            int len = Int16.Parse(step.Substring(1));
                            switch(step[0])
                            {
                                case 'U':                                    
                                    vDict[stringNum].Add(new Tuple<int,int>(x,y), len);
                                    y += len;
                                    break;
                                case 'D':                                    
                                    vDict[stringNum].Add(new Tuple<int,int>(x,y), -len);
                                    y -= len;
                                    break;
                                case 'R':                                    
                                    hDict[stringNum].Add(new Tuple<int,int>(x,y), len);
                                    x += len;
                                    break;
                                case 'L':    
                                    hDict[stringNum].Add(new Tuple<int,int>(x,y), -len);
                                    x -= len;                                
                                    break;
                            }
                        }

                        stringNum++;

                    }
                    
                    // Now find the crossing points of all the lines and store the shortest Manhattan distance
                    int shortestDist = 2000;
                    foreach(Tuple<int,int> vp in vDict[0].Keys)
                    {
                        foreach(Tuple<int,int> hp in hDict[1].Keys)
                        {
                            if (theyCross(vp, vDict[0][vp], hp, hDict[1][hp]))
                            {
                                Tuple<int,int> crossPoint = findCrossPoint(vp, vDict[0][vp], hp, hDict[1][hp]);
                                int manDist = Math.Abs(crossPoint.Item1) + Math.Abs(crossPoint.Item2);
                                if (manDist > 0 && manDist < shortestDist)
                                    shortestDist = manDist;
                            }
                        }
                    }

                    foreach(Tuple<int,int> vp in vDict[1].Keys)
                    {
                        foreach(Tuple<int,int> hp in hDict[0].Keys)
                        {
                            if (theyCross(vp, vDict[1][vp], hp, hDict[0][hp]))
                            {
                                Tuple<int,int> crossPoint = findCrossPoint(vp, vDict[1][vp], hp, hDict[0][hp]);
                                int manDist = Math.Abs(crossPoint.Item1) + Math.Abs(crossPoint.Item2);
                                if (manDist > 0 && manDist < shortestDist)
                                    shortestDist = manDist;
                            }
                        }
                    }


                    Console.WriteLine($"Answer is -not solved-");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                return;
            }
        }

        private static Tuple<int, int> findCrossPoint(Tuple<int, int> vp, int v1, Tuple<int, int> hp, int v2)
        {
            return new Tuple<int, int>(vp.Item1, hp.Item2);
        }

        private static bool theyCross(Tuple<int, int> vp, int vpLen, Tuple<int, int> hp, int hpLen)
        {
            if (Math.Min(hp.Item1, hp.Item1+hpLen) > vp.Item1)
                return false;        
            if (Math.Max(hp.Item1, hp.Item1+hpLen) < vp.Item1)
                return false;        
            if (Math.Min(vp.Item2, vp.Item2 + vpLen) > hp.Item2)    
                return false;
            if (Math.Max(vp.Item2, vp.Item2 + vpLen) < hp.Item2)    
                return false;
            
            return true;
        }
    }
}
