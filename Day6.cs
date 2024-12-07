using AOC_2024;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024;


internal class Guard(int posX, int posY, int direction)
{
    public int PosX { get; private set; } = posX;
    public int PosY { get; private set; } = posY;

    /*
     * Directions in clockwise turn (like default rotation): 0 = up, 1 = right, 2 = down, 3 = left
     */
    public int Direction { get; private set; } = direction;

    static readonly Dictionary<int, int[]> directionToMove = new()
    {
        { 0, [-1, 0]},
        { 1, [0, 1]},
        { 2, [1, 0]},
        { 3, [0, -1]},
    };

    internal bool OneStepForwardIsOnScreen(int[][] room)
    {
        int[] move = directionToMove[Direction];
        int movedX = PosX + move[0];
        int movedY = PosY + move[1];
        return movedX >= 0 && movedX < room.Length
            && movedY >= 0 && movedY < room[movedX].Length;
    }

    internal HashSet<Tuple<int, int>> MoveUntilObstacleOrEndOfScreen(int[][] room)
    {
        HashSet<Tuple<int, int>> positions = [];
        int[] move = directionToMove[Direction];
        while (OneStepForwardIsOnScreen(room) && room[PosX + move[0]][PosY + move[1]] == 0)
        {
            PosX += move[0];
            PosY += move[1];

            positions.Add(new(PosX, PosY));
        }
        return positions;
    }

    internal void Rotate()
    {
        this.Direction = (Direction + 1) % 4;
    }
}

internal class InputDay6(Guard guard, int[][] room)
{
    public Guard Guard { get; } = guard;
    public int[][] Room { get; } = room;

    private static readonly Dictionary<char, int> characterToDirection = new()
    {
        {'^', 0},
        {'>', 1 },
        {'v', 2 },
        {'<', 3 }
    };

    internal static InputDay6 Parse(string pathToFile)
    {
        string[] lines = File.ReadAllLines(pathToFile);
        Guard guard = null;
        int[][] room = new int[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            room[i] = new int[line.Length];

            for (int j = 0; j < line.Length; j++)
            {
                char character = line[j];
                if (character == '.')
                {
                    room[i][j] = 0;
                }
                else if (character == '#')
                {
                    room[i][j] = 1;
                }
                else
                {
                    guard = new Guard(i, j, characterToDirection[character]);
                }
            }
        }
        return new InputDay6(guard, room);
    }
}

internal class Day6Solver
{
    internal int SolvePart1(InputDay6 input)
    {
        Guard guard = input.Guard;
        HashSet<Tuple<int, int>> distinctPositions = [new(guard.PosX, guard.PosY)];
        int[][] room = input.Room;

        bool touchedEdgeOfScreen = false;
        while (!touchedEdgeOfScreen)
        {
            var newPositions = guard.MoveUntilObstacleOrEndOfScreen(room);
            foreach (var pos in newPositions)
            {
                distinctPositions.Add(pos);
            }
            touchedEdgeOfScreen = !guard.OneStepForwardIsOnScreen(room);
            guard.Rotate();
        }
        return distinctPositions.Count;
    }
}

