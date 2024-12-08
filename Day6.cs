using AOC_2024;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024;


internal class InputDay6(InputGuard guard, int[][] room)
{
    public InputGuard Guard { get; } = guard;
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
        InputGuard guard = null;
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
                    guard = new InputGuard(i, j, characterToDirection[character]);
                }
            }
        }
        return new InputDay6(guard, room);
    }
}

internal class InputGuard(int posX, int posY, int direction)
{
    public int PosX { get; } = posX;
    public int PosY { get; } = posY;
    public int Direction { get; } = direction;

    internal Guard Create()
    {
        return new Guard(PosX, PosY, Direction);
    }
}

internal class Guard(int posX, int posY, int direction)
{
    public int PosX { get; private set; } = posX;
    public int PosY { get; private set; } = posY;

    private readonly List<Tuple<int, int>> lastObstacles = [];

    /*
     * 0 = up, 1 = right, 2 = down, 3 = left
     */
    public int Direction { get; private set; } = direction;

    static readonly Dictionary<int, int[]> directionToMove = new()
    {
        { 0, [-1, 0]},
        { 1, [0, 1]},
        { 2, [1, 0]},
        { 3, [0, -1]},
    };

    internal bool NextIsOnScreen(int[][] room)
    {
        int[] move = directionToMove[Direction];
        int movedX = PosX + move[0];
        int movedY = PosY + move[1];
        return movedX >= 0 && movedX < room.Length
            && movedY >= 0 && movedY < room[movedX].Length;
    }

    internal bool NextIsFree(int[][] room)
    {
        if (!NextIsOnScreen(room))
        {
            throw new Exception("What ?");
        }
        int[] move = directionToMove[Direction];
        int movedX = PosX + move[0];
        int movedY = PosY + move[1];
        return room[movedX][movedY] == 0;
    }

    internal HashSet<Tuple<int, int>> MoveUntilObstacleOrEndOfScreen(int[][] room)
    {
        HashSet<Tuple<int, int>> positions = [];
        int[] move = directionToMove[Direction];
        while (NextIsOnScreen(room) && room[PosX + move[0]][PosY + move[1]] == 0)
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

    internal bool FindNextObstacleCandidate(int[][] room, out Tuple<int, int> obstacleCandidate)
    {
        int obstacleCandidatePosX = -1;
        int obstacleCandidatePosY = -1;

        while (NextIsOnScreen(room))
        {
            int[] move = directionToMove[Direction];
            var nextX = PosX + move[0];
            var nextY = PosY + move[1];
            if (room[nextX][nextY] == 0)
            {
                PosX = nextX;
                PosY = nextY;
                Console.WriteLine($"Moving to: ({PosX}, {PosY})");
                if (obstacleCandidatePosX > 0
                    && PosX == obstacleCandidatePosX && PosY == obstacleCandidatePosY)
                {
                    Console.WriteLine($"--- --- Functioning obstacle: ({PosX}, {PosY})");
                    // we knew it would be creating a cycle and now know it's reachable
                    obstacleCandidate = new(obstacleCandidatePosX, obstacleCandidatePosY);
                    // remove from first till third-to-last, only keep two
                    lastObstacles.RemoveRange(0, lastObstacles.Count - 2);
                    return true;
                }
            }
            else if (room[nextX][nextY] == 1)
            {
                Console.WriteLine($"Real obstacle: ({nextX}, {nextY})");
                lastObstacles.Add(new(nextX, nextY));

                if (lastObstacles.Count >= 3)
                {
                    // we can close the last three with a fourth by taking a diagonal
                    var lastObstacle = lastObstacles[lastObstacles.Count - 1];
                    var thirdToLastObstacle = lastObstacles[lastObstacles.Count - 3];

                    int[] nextStep = directionToMove[(Direction + 1) % 4];

                    if (PosX == nextX)
                    {
                        obstacleCandidatePosY = PosY;
                        obstacleCandidatePosX = thirdToLastObstacle.Item1 + nextStep[0];
                    }
                    else if (PosY == nextY)
                    {
                        obstacleCandidatePosX = PosX;
                        obstacleCandidatePosY = thirdToLastObstacle.Item2 + nextStep[1];
                    }
                    Console.WriteLine($"--- Candidate obstacle: ({obstacleCandidatePosX}, {obstacleCandidatePosY})");
                }
                Rotate();
            }
        }
        obstacleCandidate = new(-1, -1);
        return false;
    }
}


internal class Day6Solver
{
    internal int SolvePart1(InputDay6 input)
    {
        Guard guard = input.Guard.Create();
        int[][] room = input.Room;

        HashSet<Tuple<int, int>> distinctPositions = [new(guard.PosX, guard.PosY)];
        bool touchedEdgeOfScreen = false;
        while (!touchedEdgeOfScreen)
        {
            var newPositions = guard.MoveUntilObstacleOrEndOfScreen(room);
            foreach (var pos in newPositions)
            {
                distinctPositions.Add(pos);
            }
            touchedEdgeOfScreen = !guard.NextIsOnScreen(room);
            guard.Rotate();
        }
        return distinctPositions.Count;
    }

    internal int SolvePart2(InputDay6 input)
    {
        Guard guard = input.Guard.Create();
        int[][] room = input.Room;

        var obstaclePositions = new HashSet<Tuple<int, int>>();
        while (guard.NextIsOnScreen(room))
        {
            if (guard.FindNextObstacleCandidate(room, out var obstacleCandidate))
            {
                obstaclePositions.Add(obstacleCandidate);
            }
        }

        return obstaclePositions.Count;
    }
}

