using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var timer = new Stopwatch();
            timer.Start();
            
            try
            {
                var vDict = new Dictionary<Tuple<int,int>, int>[2]{new Dictionary<Tuple<int,int>, int>(), 
                                                                   new Dictionary<Tuple<int,int>, int>()};
                var hDict = new Dictionary<Tuple<int,int>, int>[2]{new Dictionary<Tuple<int,int>, int>(), 
                                                                   new Dictionary<Tuple<int,int>, int>()};
                int stringNum = 0;
                var vCrossingDict = new Dictionary<Tuple<int,int>, Tuple<int,int>>[2]{new Dictionary<Tuple<int,int>, Tuple<int,int>>(), 
                                                                   new Dictionary<Tuple<int,int>, Tuple<int,int>>()};
                var hCrossingDict = new Dictionary<Tuple<int,int>, Tuple<int,int>>[2]{new Dictionary<Tuple<int,int>, Tuple<int,int>>(), 
                                                                   new Dictionary<Tuple<int,int>, Tuple<int,int>>()};
                using (StreamReader sr = new StreamReader("test1.txt"))
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
                    for(int wireNum = 0; wireNum <= 1; wireNum++) // only copes with 2 wires!
                    {
                        foreach(Tuple<int,int> vp in vDict[wireNum].Keys)
                        {
                            foreach(Tuple<int,int> hp in hDict[1-wireNum].Keys)
                            {
                                if (theyCross(vp, vDict[wireNum][vp], hp, hDict[1-wireNum][hp]))
                                {
                                    Tuple<int,int> crossPoint = findCrossPoint(vp, vDict[wireNum][vp], hp, hDict[1-wireNum][hp]);
                                    int manDist = Math.Abs(crossPoint.Item1) + Math.Abs(crossPoint.Item2);
                                    if (manDist > 0 && manDist < shortestDist)
                                        shortestDist = manDist;

                                    // Mark the crossing lines as intersections, with length along/up to the cross point
                                    if (manDist > 0)
                                    {
                                        int vCrossLength = Math.Abs(hp.Item2 - vp.Item2);
                                        if (!vCrossingDict[wireNum].ContainsKey(vp))
                                            vCrossingDict[wireNum].Add(vp, new Tuple<int, int>(vDict[wireNum][vp], vCrossLength));

                                        int hCrossLength = Math.Abs(vp.Item1 - hp.Item1);
                                        if (!hCrossingDict[1-wireNum].ContainsKey(hp))
                                            hCrossingDict[1-wireNum].Add(hp, new Tuple<int, int>(hDict[1-wireNum][hp], hCrossLength));
                                    }
                                }
                            }
                        }
                    }

                    // Now walk all the line segements and when an intersection is met, record the length in another dict
                    Dictionary<Tuple<int,int>, int[]> intersections = new Dictionary<Tuple<int, int>, int[] >();
                    int wireNum2 = 0;
                    while (wireNum2 < 2)
                    {
                        Tuple<int,int> currentPoint = new Tuple<int,int>(0,0);
                        int lengthWalked = 0;
                        while (true)
                        {
                            // Horizontal first 
                            // - Is this an intersection line?
                            if (hCrossingDict[wireNum2].ContainsKey(currentPoint))
                            {
                                // Yes, so only count the length to the intersection
                                int sublengthWalked = lengthWalked + hCrossingDict[wireNum2][currentPoint].Item2;
                                Tuple<int,int> intersectionPoint = new Tuple<int,int>(hCrossingDict[wireNum2][currentPoint].Item1 > 0 ? 
                                                                                                        currentPoint.Item1 + hCrossingDict[wireNum2][currentPoint].Item2 
                                                                                                        : currentPoint.Item1 - hCrossingDict[wireNum2][currentPoint].Item2, // x-coord
                                                                                                    currentPoint.Item2); // y-coord
                                if (intersections.ContainsKey(intersectionPoint))
                                {
                                    intersections[intersectionPoint] = new int[] {intersections[intersectionPoint][0], sublengthWalked};
                                }
                                else
                                {
                                    intersections.Add(intersectionPoint, new int[] {sublengthWalked, 0}); // Second param is length to this intersection for wire 2
                                }
                            }

                            // Now walk the whole of this segment and update current point                       
                            if (!hDict[wireNum2].ContainsKey(currentPoint))
                            {
                                // Does the origin line actually start vertically?
                                if (currentPoint.Item1 != 0 || currentPoint.Item2 != 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                lengthWalked += Math.Abs(hDict[wireNum2][currentPoint]);
                                currentPoint = new Tuple<int,int>(currentPoint.Item1 + hDict[wireNum2][currentPoint], currentPoint.Item2);
                            }

                            // Now vertical 
                            // - Is this an intersection line?
                            if (vCrossingDict[wireNum2].ContainsKey(currentPoint))
                            {
                                // Yes, so only count the length to the intersection
                                int sublengthWalked = lengthWalked + vCrossingDict[wireNum2][currentPoint].Item2;
                                Tuple<int,int> intersectionPoint = new Tuple<int,int>(currentPoint.Item1, // x-coord
                                                                                                vCrossingDict[wireNum2][currentPoint].Item1 > 0 ? 
                                                                                                        currentPoint.Item2 + vCrossingDict[wireNum2][currentPoint].Item2 
                                                                                                        : currentPoint.Item2 - vCrossingDict[wireNum2][currentPoint].Item2); // y-coord
                                if (intersections.ContainsKey(intersectionPoint))
                                {
                                    intersections[intersectionPoint] = new int[] {intersections[intersectionPoint][0], sublengthWalked};
                                }
                                else
                                {
                                    intersections.Add(intersectionPoint, new int[] {sublengthWalked, 0}); // Second param is length to this intersection for wire 2
                                }
                            }

                            // Now walk the whole of this segment and update curernt point                       
                            if (!vDict[wireNum2].ContainsKey(currentPoint))
                                break;

                            lengthWalked += Math.Abs(vDict[wireNum2][currentPoint]);
                            currentPoint = new Tuple<int,int>(currentPoint.Item1, currentPoint.Item2 + vDict[wireNum2][currentPoint]);

                        }
                        wireNum2++;
                    }
                    
                    // Find the intersection with the smallest sum
                    int minLength = intersections.Values.Min(x => x[0]+x[1]);

                    timer.Stop();
                    Console.WriteLine($"Answer is {minLength} in {timer.ElapsedMilliseconds}ms");
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
