using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    private long Solve(Instruction[] instructions, Func<long, long, int[], bool> recursiveCheck)
    {
        long totalScore = 0;
        foreach (Instruction instruction in instructions)
        {
            if (CanSolve(instruction, recursiveCheck))
            {
                totalScore += instruction.Result;
            }
        }
        return totalScore;
    }

    internal long SolvePart1(Instruction[] instructions)
    {
        return Solve(instructions, CanSolveRecursively1);
    }

    internal long SolvePart2(Instruction[] instructions)
    {
        return Solve(instructions, CanSolveRecursively2);
    }

    private bool CanSolve(Instruction instruction, Func<long, long, int[], bool> recursiveCheck)
    {
        long result = instruction.Result;
        int[] operands = instruction.Operands;
        long startValue = operands[0];
        int[] otherOperands = DropFirst(operands);
        return recursiveCheck(startValue, result, otherOperands);
    }

    private bool CanSolveRecursively2(long count, long result, int[] operands)
    {
        if (operands.Length == 0)
        {
            return count == result;
        }
        int nextOperand = operands[0];
        int[] reducedOperands = DropFirst(operands);

        long sum = count + nextOperand;
        long product = count * nextOperand;
        long concatenation = long.Parse("" + count + nextOperand);

        return CanSolveRecursively2(sum, result, reducedOperands)
            || CanSolveRecursively2(product, result, reducedOperands)
            || CanSolveRecursively2(concatenation, result, reducedOperands);
    }

    private bool CanSolveRecursively1(long count, long result, int[] operands)
    {
        if (operands.Length == 0)
        {
            return count == result;
        }
        int nextOperand = operands[0];
        int[] reducedOperands = DropFirst(operands);

        long sum = count + nextOperand;
        long product = count * nextOperand;

        return CanSolveRecursively1(sum, result, reducedOperands) || CanSolveRecursively1(product, result, reducedOperands);
    }

    private int[] DropFirst(int[] array)
    {
        return array.Where((instruction, index) => index != 0).ToArray();
    }
}