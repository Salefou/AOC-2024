using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC_2024;

internal class InputDay7(Instruction[] instructions)
{
    public Instruction[] Instructions { get; } = instructions;

    internal static InputDay7 Parse(string pathToFile)
    {
        string[] lines = File.ReadAllLines(pathToFile);
        List<Instruction> instructions = new();
        foreach (string line in lines)
        {
            string resultPattern = @"(-?\d+):";
            string value = Regex.Match(line, resultPattern).Value;
            string valueAsString = value.Substring(0, value.Length - 1);
            long result = long.Parse(valueAsString);

            string operandPattern = @" (-?\d+)";
            MatchCollection matchCollection = Regex.Matches(line, operandPattern);
            int[] operands = new int[matchCollection.Count];
            int index = 0;
            foreach (Match match in matchCollection)
            {
                operands[index] = int.Parse(match.Value);
                index++;
            }
            Instruction instruction = new(result, operands);
            instructions.Add(instruction);
        }
        return new InputDay7([.. instructions]);
    }
}

internal class Instruction(long result, int[] operands)
{
    public long Result { get; } = result;
    public int[] Operands { get; } = operands;
}

internal class Day7Solver
{
    internal long SolvePart1(Instruction[] instructions)
    {
        long totalScore = 0;
        foreach (Instruction instruction in instructions)
        {
            if (CanSolve(instruction))
            {
                totalScore += instruction.Result;
            }
        }
        return totalScore;
    }

    private bool CanSolve(Instruction instruction)
    {
        long result = instruction.Result;
        int[] operands = instruction.Operands;
        long startValue = operands[0];
        int[] otherOperands = DropFirst(operands);
        return CanSolveRecursively(startValue, result, otherOperands);
    }

    private bool CanSolveRecursively(long count, long result, int[] operands)
    {
        if (operands.Length == 0)
        {
            return count == result;
        }
        int nextOperand = operands[0];
        int[] reducedOperands = DropFirst(operands);

        long sum = count + nextOperand;
        long product = count * nextOperand;

        return CanSolveRecursively(sum, result, reducedOperands) || CanSolveRecursively(product, result, reducedOperands);
    }

    private int[] DropFirst(int[] array)
    {
        return array.Where((instruction, index) => index != 0).ToArray();
    }
}