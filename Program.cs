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
            InputDay4 inputDay4Part1 = InputDay4.Parse("../../inputs/input_day_4.txt", new HashSet<char>() { 'X', 'M', 'A', 'S' });
            Day4Solver day4Solver = new();

            int solutionPart1 = day4Solver.SolvePart1(inputDay4Part1);
            Console.WriteLine(solutionPart1);

            InputDay4 inputDay4Part2 = InputDay4.Parse("../../inputs/input_day_4.txt", new HashSet<char>() { 'M', 'A', 'S' });
            //int solutionPart2 = day3Solver.SolvePart2(inputDay3);
            //Console.WriteLine(solutionPart2);

            Console.ReadLine();
        }
    }
}
