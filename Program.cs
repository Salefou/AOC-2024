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
            InputDay2 inputDay2 = InputDay2.Parse("../../inputs/input_day_2.txt");
            Day2Solver day2Solver = new();

            int solutionPart1 = day2Solver.SolvePart1(inputDay2);
            Console.WriteLine(solutionPart1);

            int solutionPart2 = day2Solver.SolvePart2(inputDay2);
            Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
