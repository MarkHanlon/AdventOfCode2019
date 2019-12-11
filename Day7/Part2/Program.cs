using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day7
{

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            string programCode = "";            
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                programCode = sr.ReadToEnd();                    
            }

            List<List<int>> allPerms = FindPermutations(new List<int>{5,6,7,8,9});
            int highestOutput = 0;
            
            foreach (List<int> thisPerm in allPerms)
            //List<int> thisPerm = new List<int> {9,8,7,6,5};
            {          
                // Try a new permutation - reset the computers!                      
                var computers = new IntcodeComputer[] {new IntcodeComputer(), new IntcodeComputer(), new IntcodeComputer(), new IntcodeComputer(), new IntcodeComputer()};
                for (int i = 0; i <= 4; i++)
                {
                    computers[i].Load(programCode);
                    computers[i].AddInput(thisPerm[i]);
                }

                int input = 0;
                bool running = true;
                while (running)
                {
                    for (int i = 0; i < thisPerm.Count(); i++)
                    {                                                                                
                        computers[i].AddInput(input);
                        int output = computers[i].Run();
                        if (computers[i].IsFinished)
                            running = false;

                        input = output;
                    }
                }

                // All programs finished, is the answer the best yet?
                if (input > highestOutput)
                    highestOutput = input;
                
            }
            timer.Stop();
            Console.WriteLine($"Done with output: {highestOutput}, execution in {timer.ElapsedMilliseconds}ms");
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
        int[] _ram;
        int _pc;
        bool _isFinished;

        Queue<int> _inputs;
        int _output;

        public void Load(string program)
        {
            _program = program;
            _inputs = new Queue<int>();
            _output = -1;
          
            Reset();
        }

        public void AddInput(int input)
        {
            _inputs.Enqueue(input);
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
            
            _ram = ramList.ToArray();
            _pc = 0;
            IsFinished = false;
        }

        public bool IsFinished { get; private set; }
        public int Run()
        {
            try
            {                                
                while (_ram[_pc] != 99)
                {
                    // Extract the op-code
                    int opCode = _ram[_pc] % 100;
                    int[] pMode = new int[10];   // 0 = position mode, 1 = immediate mode
                    int i = 0;
                    foreach (char c in _ram[_pc].ToString().Reverse().Skip(2))
                    {
                        pMode[i++] = Int16.Parse(c.ToString());
                    }

                    switch (opCode)
                    {
                        case 1: // Add [p1] + [p2] -> [p3]
                            _ram[_ram[_pc+3]] = DecodeParam(pMode[0], _pc+1) + DecodeParam(pMode[1], _pc+2);
                            _pc+=4;
                            break;
                        case 2: // Multiply [p1] * [p2] -> [p3]
                            _ram[_ram[_pc+3]] = DecodeParam(pMode[0], _pc+1) * DecodeParam(pMode[1], _pc+2);
                            _pc+=4;
                            break;
                        case 3: // Input -> [p1]
                            if (_inputs.Count > 0)
                            {
                                Console.WriteLine($"Input required: ({_inputs.Peek()})");                            
                                _ram[_ram[_pc+1]] = _inputs.Dequeue();
                                _pc+=2;
                            }
                            else
                            {
                                // Need to return to await a next input, via 'Continue'
                                Console.WriteLine("Input required: <empty - awaiting input>");
                                return _output;
                            }
                            break;
                        case 4: // Output <- [p1]
                            _output = DecodeParam(pMode[0], _pc+1);
                            Console.WriteLine($"Output: {_output}");
                            _pc+=2;
                            break;
                        case 5: // Jump if true (if [p1] != 0 [p2] -> pc)
                            if (DecodeParam(pMode[0], _pc+1) != 0)
                                _pc = DecodeParam(pMode[1], _pc+2);
                            else
                                _pc+=3;
                            break;
                        case 6: // Jump if false (if [p1] == 0 [p2] -> pc)
                            if (DecodeParam(pMode[0], _pc+1) == 0)
                                _pc = DecodeParam(pMode[1], _pc+2);
                            else
                                _pc+=3;
                            break;
                        case 7: //less than (if [p1] < [p2] 1 => [p3], else 0 => [p3])
                            if (DecodeParam(pMode[0], _pc+1) < DecodeParam(pMode[1], _pc+2))
                                _ram[_ram[_pc+3]] = 1;
                            else
                                _ram[_ram[_pc+3]] = 0;
                            _pc+=4;
                            break;
                        case 8: //equals (if [p1] == [p2] 1 => [p3], else 0 => [p3])
                            if (DecodeParam(pMode[0], _pc+1) == DecodeParam(pMode[1], _pc+2))
                                _ram[_ram[_pc+3]] = 1;
                            else
                                _ram[_ram[_pc+3]] = 0;
                            _pc+=4;
                            break;
                        default:
                            throw new Exception("Unknown command - program terminated");
                    }
                    
                }

                // Done
                Console.WriteLine($"Complete: Outputs returned: {string.Join(',', _output)}");
                IsFinished = true;
                return _output;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                return -99;
            }
        }
        private int DecodeParam(int paramMode, int paramAddr)
        {
            return (paramMode == 0 ? _ram[_ram[paramAddr]] : _ram[paramAddr]);
        }


    }
}
