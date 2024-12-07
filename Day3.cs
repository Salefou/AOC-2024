using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC_2024;

internal class InputDay3(string[] validInstructions)
{
    public string[] ValidInstructions { get; } = validInstructions;

    internal static InputDay3 Parse(string pathToFile)
    {
        string rawInput = File.ReadAllText(pathToFile);
        string pattern = @"mul\((-?\d+),\s*(-?\d+)\)|don't\(\)|do\(\)";
        MatchCollection matches = Regex.Matches(rawInput, pattern);
        string[] validInstructions = new string[matches.Count];
        int index = 0;
        foreach (Match match in matches)
        {
            var instruction = match.Value;
            validInstructions[index] = instruction;
            index++;
        }
        return new InputDay3(validInstructions);
    }
}

internal class Day3Solver
{
    public int SolvePart1(InputDay3 input)
    {
        int totalScore = 0;
        foreach (var instruct in input.ValidInstructions)
        {
            if (IsMul(instruct))
            {
                totalScore += ComputeInstruction(instruct);
            }
        }
        return totalScore;
    }

    public int SolvePart2(InputDay3 input)
    {
        bool enabled = true;
        int totalScore = 0;
        foreach (var instruct in input.ValidInstructions)
        {
            if (IsActivation(instruct))
            {
                enabled = true;
            }
            else if (IsDeactivation(instruct))
            {
                enabled = false;
            }
            else if (enabled && IsMul(instruct))
            {
                totalScore += ComputeInstruction(instruct);
            }
        }
        return totalScore;
    }

    private bool IsMul(string validInstruction)
    {
        return validInstruction.Substring(0, 3) == "mul";
    }

    private bool IsActivation(string validInstruction)
    {
        return validInstruction == "do()";
    }

    private bool IsDeactivation(string validInstruction)
    {
        return validInstruction == "don't()";
    }

    private int ComputeInstruction(string validInstruction)
    {
        // skip 4 chars for "mul(" and a total of 5 to ignore last ")"
        int[] numbers = validInstruction.Substring(4, validInstruction.Length - 5)
            .Split(',')
            .Select(int.Parse)
            .ToArray();
        if (numbers.Count() > 2)
        {
            throw new ArgumentException();
        }
        return numbers[0] * numbers[1];
    }

}

