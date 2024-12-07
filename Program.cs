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
            string path = "../../inputs/input_day_6.txt";
            InputDay6 input = InputDay6.Parse(path);

            Day6Solver solver = new();
            int solutionPart1 = solver.SolvePart1(input);
            Console.WriteLine(solutionPart1);

            //int solutionPart2 = day5Solver.SolvePart2(input);
            //Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
