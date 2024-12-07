using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024;
internal class Day1
{
    static void Main(string[] args)
    {
        InputDay1 inputDay1 = InputDay1.Parse("../../inputs/input_day_1.txt");
        Day1Solver day1Solver = new Day1Solver();
        day1Solver.SolvePart1(inputDay1);
        day1Solver.SolvePart2(inputDay1);
        Console.ReadLine();
    }
}

internal class InputDay1(int[] left, int[] right)
{
    public int[] Left { get; } = left;
    public int[] Right { get; } = right;

    internal static InputDay1 Parse(string pathToFile)
    {
        var lines = File.ReadAllLines(pathToFile);
        var firstArray = lines.Select(line => int.Parse(line.Split()[0])).ToArray();
        var secondArray = lines.Select(line => int.Parse(line.Split()[3])).ToArray();
        return new InputDay1(firstArray, secondArray);
    }
}

internal class Day1Solver
{

    public void SolvePart1(InputDay1 input)
    {
        int[] leftSorted = input.Left.OrderBy(myInt => myInt).ToArray();
        int[] rightSorted = input.Right.OrderBy(myInt => myInt).ToArray();
        int size = input.Left.Length;
        int totalScore = 0;
        for (int i = 0; i < size; i++)
        {
            totalScore += Math.Abs(leftSorted[i] - rightSorted[i]);
        }

        Console.WriteLine(totalScore);
    }

    public void SolvePart2(InputDay1 input)
    {

        Dictionary<int, int> leftFreq = FrequencyMap(input.Left);
        Dictionary<int, int> rightFreq = FrequencyMap(input.Right);
        int totalScore = 0;
        foreach (var elementToLeftFreq in leftFreq)
        {
            var element = elementToLeftFreq.Key;

            if (!rightFreq.TryGetValue(element, out int freqInRight))
            {
                freqInRight = 0;
            }

            totalScore += element * elementToLeftFreq.Value * freqInRight;
        }
        Console.WriteLine(totalScore);
    }

    private Dictionary<int, int> FrequencyMap(int[] input)
    {
        Dictionary<int, int> frequencies = [];
        foreach (int element in input)
        {
            if (frequencies.TryGetValue(element, out int value))
            {
                frequencies[element] = value + 1;
            }
            else
            {
                frequencies[element] = 1;
            }
        }
        return frequencies;
    }
}


