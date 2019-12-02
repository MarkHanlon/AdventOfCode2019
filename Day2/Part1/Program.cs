using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{

    class Program
    {
        static void Main(string[] args)
        {

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

                    // Follow the instructions!
                    ram[1] = 12;
                    ram[2] = 2;

                    int pc = 0;
                    while (ram[pc] != 99)
                    {
                        switch (ram[pc])
                        {
                            case 1:
                                ram[ram[pc+3]] = ram[ram[pc+1]] + ram[ram[pc+2]];
                                pc+=4;
                                break;
                            case 2:
                                ram[ram[pc+3]] = ram[ram[pc+1]] * ram[ram[pc+2]];
                                pc+=4;
                                break;
                            default:
                                throw new Exception("Unknown command - program terminated");
                        }
                        
                    }

                    // Now return the value at memory location 0
                    Console.WriteLine($"Answer is {ram[0]}");
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
