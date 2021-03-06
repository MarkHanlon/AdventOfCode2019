﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day7
{

    class Program
    {
        public const int WIDTH = 25;
        public const int HEIGHT = 6;
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            string imageData = "";            
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                imageData = sr.ReadToEnd();                    
            }

            int[,,] image = new int[100,WIDTH,HEIGHT]; // layer, width, height
            int x = 0, y = 0, layer = 0;
            foreach(char c in imageData)
            {
                image[layer,x, y] = Int32.Parse(c.ToString());
                x++;
                if (x >= WIDTH)
                {
                    x = 0;
                    y++;
                    if (y >= HEIGHT)
                    {
                        y = 0;
                        layer++;
                    }
                }
            }

            int layerWithLowestZeros = -1;
            int lowestZeros = WIDTH * HEIGHT;
            for (int lay = 0; lay < layer; lay++)
            {
                int numberOfZeros = CountLayer(image, lay, 0);
                if (numberOfZeros < lowestZeros)
                {
                    lowestZeros = numberOfZeros;
                    layerWithLowestZeros = lay;
                }
            }

            int numberOfOnes = CountLayer(image, layerWithLowestZeros, 1);
            int numberOfTwos = CountLayer(image, layerWithLowestZeros, 2);

            timer.Stop();
            Console.WriteLine($"Done with output: {numberOfOnes * numberOfTwos}, execution in {timer.ElapsedMilliseconds}ms");

        }

        private static int CountLayer(int[,,] image, int lay, int v)
        {
            int count = 0;

            for(int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (image[lay, x, y] == v)
                        count++;
                }
            return count;
        }

        static List<List<int>> FindPermutations(List<int> input)
        {
            if (input.Count() == 1)
                return new List<List<int>> { input };
            var perms = new List<List<int>>();
            foreach (int i in input)
            {
                var others = input.Except(new List<int>{i});               
                perms.AddRange(FindPermutations(others.ToList()).Select(s => s.Prepend(i).ToList()));
            }
            return perms;
        }
    }

    class IntcodeComputer
    {
        private string _program;
        int[] ram;

        public void Load(string program)
        {
            _program = program;
            Reset();
        }

        public void Reset()
        {
            // Clear ram
            var ramList = new List<int>(); 
            var commandStrings = _program.Split(',');
            foreach(string command in commandStrings)
            {
                ramList.Add(Int32.Parse(command));
            }
            
            ram = ramList.ToArray();
        }

        public int[] Run(int[] inputs)
        {
            try
            {
                List<int> outputs = new List<int>();
                int nextInput = 0;

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
                            ram[ram[pc+3]] = DecodeParam(pMode[0], pc+1) + DecodeParam(pMode[1], pc+2);
                            pc+=4;
                            break;
                        case 2: // Multiply [p1] * [p2] -> [p3]
                            ram[ram[pc+3]] = DecodeParam(pMode[0], pc+1) * DecodeParam(pMode[1], pc+2);
                            pc+=4;
                            break;
                        case 3: // Input -> [p1]
                            Console.WriteLine($"Input required: ({inputs[nextInput]})");                            
                            ram[ram[pc+1]] = inputs[nextInput++];
                            pc+=2;
                            break;
                        case 4: // Output <- [p1]
                            outputs.Add(DecodeParam(pMode[0], pc+1));
                            Console.WriteLine($"Output: {outputs.Last()}");
                            pc+=2;
                            break;
                        case 5: // Jump if true (if [p1] != 0 [p2] -> pc)
                            if (DecodeParam(pMode[0], pc+1) != 0)
                                pc = DecodeParam(pMode[1], pc+2);
                            else
                                pc+=3;
                            break;
                        case 6: // Jump if false (if [p1] == 0 [p2] -> pc)
                            if (DecodeParam(pMode[0], pc+1) == 0)
                                pc = DecodeParam(pMode[1], pc+2);
                            else
                                pc+=3;
                            break;
                        case 7: //less than (if [p1] < [p2] 1 => [p3], else 0 => [p3])
                            if (DecodeParam(pMode[0], pc+1) < DecodeParam(pMode[1], pc+2))
                                ram[ram[pc+3]] = 1;
                            else
                                ram[ram[pc+3]] = 0;
                            pc+=4;
                            break;
                        case 8: //equals (if [p1] == [p2] 1 => [p3], else 0 => [p3])
                            if (DecodeParam(pMode[0], pc+1) == DecodeParam(pMode[1], pc+2))
                                ram[ram[pc+3]] = 1;
                            else
                                ram[ram[pc+3]] = 0;
                            pc+=4;
                            break;
                        default:
                            throw new Exception("Unknown command - program terminated");
                    }
                    
                }

                // Done
                Console.WriteLine($"Complete: Outputs returned: {string.Join(',', outputs.ToArray())}");
                return outputs.ToArray();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                return null;
            }
        }
        private int DecodeParam(int paramMode, int paramAddr)
        {
            return (paramMode == 0 ? ram[ram[paramAddr]] : ram[paramAddr]);
        }


    }
}
