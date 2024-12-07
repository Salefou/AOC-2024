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
            string path = "../../inputs/input_day_5.txt";
            InputDay5 input = InputDay5.Parse(path);

            Day5Solver day5Solver = new();
            int solutionPart1 = day5Solver.SolvePart1(input);
            Console.WriteLine(solutionPart1);

            Console.ReadLine();
        }
    }
}
