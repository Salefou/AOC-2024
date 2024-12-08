using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string path = "../../inputs/input_day_7.txt";
            var input = InputDay7.Parse(path);

            Day7Solver solver = new();
            Instruction[] instructions = input.Instructions;
            var solutionPart1 = solver.SolvePart1(instructions);
            Console.WriteLine(solutionPart1);

            var solutionPart2 = solver.SolvePart2(instructions);
            Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
