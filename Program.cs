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
            string path = "../../inputs/input_day_4.txt";
            Day4Solver day4Solver = new();

            InputDay4 input = InputDay4.Parse(path);
            int solutionPart1 = day4Solver.SolvePart1(input);
            Console.WriteLine(solutionPart1);

            int solutionPart2 = day4Solver.SolvePart2(input);
            Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
