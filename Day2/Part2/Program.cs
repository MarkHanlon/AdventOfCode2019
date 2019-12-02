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
                int noun = 0;
                int verb = 0;
                do
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
                        ram[1] = noun;
                        ram[2] = verb;

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

                        // Now check value at address 0 is 19690720
                        if (ram[0] == 19690720)
                        {
                            Console.WriteLine($"DONE!! Answer is {100 * noun + verb}");
                            break;
                        }

                        verb++;
                        if (verb > 99)
                        {
                            verb = 0;
                            noun++;
                        }

                    }
                } while(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                return;
            }
        }

    }
}
