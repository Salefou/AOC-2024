using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024;

enum Characters
{
    X = 0, M = 1, A = 2, S = 3, O = -1
}

internal class InputDay4(Characters[][] matrix)
{
    private static readonly Dictionary<char, Characters> mapping = new()
    {
        { 'X', Characters.X },
        { 'M', Characters.M },
        { 'A', Characters.A },
        { 'S', Characters.S },
    };

    public Characters[][] Matrix { get; } = matrix;

    internal static InputDay4 Parse(string pathToFile)
    {
        string[] lines = File.ReadAllLines(pathToFile);
        Characters[][] matrix = new Characters[lines.Length][];
        int lineIndex = 0;
        foreach (string line in lines)
        {
            matrix[lineIndex] = line.Select(
                myChar => mapping.ContainsKey(myChar) ? mapping[myChar] : Characters.O
                ).ToArray();
            lineIndex++;
        }
        return new InputDay4(matrix);
    }
}

internal class Day4Solver
{
    internal int SolvePart1(InputDay4 input)
    {
        int totalXmasCount = 0;
        Characters[][] matrix = input.Matrix;
        for (int i = 0; i < matrix.Length; i++)
        {
            Characters[] line = input.Matrix[i];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == Characters.X)
                {
                    foreach (var move in allMoves)
                    {
                        totalXmasCount += SearchDirection(matrix, move, i, j, (int)Characters.X);
                    }
                }
            }
        }
        return totalXmasCount;
    }

    internal int SolvePart2(InputDay4 input)
    {
        int totalXmasCount = 0;
        Characters[][] matrix = input.Matrix;
        for (int i = 0; i < matrix.Length; i++)
        {
            Characters[] line = input.Matrix[i];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == Characters.A)
                {
                    if (FindTwoCrossesThrough(matrix, i, j))
                    {
                        totalXmasCount++;
                    }
                }
            }
        }
        return totalXmasCount;
    }

    private bool FindTwoCrossesThrough(Characters[][] matrix, int i, int j)
    {
        int totalCrossesThrough = 0;
        foreach (var crossMove in crossMoves)
        {
            int movedI = i + crossMove[0];
            int movedJ = j + crossMove[1];
            if (TryGet(movedI, movedJ, matrix, out var characters))
            {
                if (characters == Characters.M)
                {
                    int[] reverseMove = [-crossMove[0], -crossMove[1]];
                    totalCrossesThrough += SearchDirection(matrix, reverseMove, movedI, movedJ, (int)Characters.M);
                    if (totalCrossesThrough >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static readonly int[][] allMoves = [
        [-1, 1], [0, 1], [1, 1],
        [-1, 0], /*own*/ [1, 0],
        [-1, -1], [0, -1], [1, -1],
    ];

    private static readonly int[][] crossMoves = [
        [-1, -1], [1, -1],
        [-1, 1], [1, 1],
    ];

    private int SearchDirection(Characters[][] matrix, int[] move, int i, int j, int current)
    {
        int movedI = move[0] + i;
        int movedJ = move[1] + j;
        if (TryGet(movedI, movedJ, matrix, out var characters))
        {
            if (((int)characters) == current + 1)
            {
                if (current + 1 == (int)Characters.S)
                {
                    return 1;
                }

                return SearchDirection(matrix, move, movedI, movedJ, current + 1);
            }
        }

        return 0;
    }

    private bool TryGet(int i, int j, Characters[][] matrix, out Characters characters)
    {
        if (i >= 0 && i < matrix.Length
            && j >= 0 && j < matrix[i].Length)
        {
            characters = matrix[i][j];
            return true;
        }
        characters = Characters.O;
        return false;
    }
}

