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
            InputDay3 inputDay3 = InputDay3.Parse("../../inputs/input_day_3.txt");
            Day3Solver day3Solver = new();

            int solutionPart1 = day3Solver.SolvePart1(inputDay3);
            Console.WriteLine(solutionPart1);

            int solutionPart2 = day3Solver.SolvePart2(inputDay3);
            Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
