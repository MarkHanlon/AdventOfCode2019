using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day5
{

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            string programCode = "";
            List<int> ramList = new List<int>();

            try
            {
                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    programCode = sr.ReadToEnd();                    

                    var commandStrings = programCode.Split(',');
                    foreach(string command in commandStrings)
                    {
                        ramList.Add(Int32.Parse(command));
                    }

                    // Work with the list as an array
                    int[] ram = ramList.ToArray();

                    int pc = 0;
                    while (ram[pc] != 99)
                    {
                        // Extract the op-code
                        int opCode = ram[pc] % 100;
                        int[] pMode = new int[10];   // 0 = position mode, 1 = immediate mode
                        int i = 0;
                        foreach (char c in ram[pc].ToString().Reverse().Skip(2))
                        {
                            pMode[i++] = Int16.Parse(c.ToString());
                        }

                        switch (opCode)
                        {
                            case 1: // Add [p1] + [p2] -> [p3]
                                //int a = pMode[0] == 0 ? ram[ram[pc+1]] : ram[pc+1];
                                //int b = pMode[1] == 0 ? ram[ram[pc+2]] : ram[pc+2];
                                //ram[ram[pc+3]] = a + b;
                                ram[ram[pc+3]] = (pMode[0] == 0 ? ram[ram[pc+1]] : ram[pc+1]) +
                                                 (pMode[1] == 0 ? ram[ram[pc+2]] : ram[pc+2]);
                                pc+=4;
                                break;
                            case 2: // Multiply [p1] * [p2] -> [p3]
                                ram[ram[pc+3]] = (pMode[0] == 0 ? ram[ram[pc+1]] : ram[pc+1]) *
                                                 (pMode[1] == 0 ? ram[ram[pc+2]] : ram[pc+2]);
                                pc+=4;
                                break;
                            case 3: // Input -> [p1]
                                Console.Write("Input required: ");
                                string param = "1"; //Console.ReadLine();
                                ram[ram[pc+1]] = Int32.Parse(param);
                                pc+=2;
                                break;
                            case 4: // Output <- [p1]
                                Console.WriteLine($"Output: {(pMode[0] == 0 ? ram[ram[pc+1]] : ram[pc+1])}");
                                pc+=2;
                                break;

                            default:
                                throw new Exception("Unknown command - program terminated");
                        }
                        
                    }

                    // Done
                    timer.Stop();
                    Console.WriteLine($"Done execution in {timer.ElapsedMilliseconds}ms");
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
