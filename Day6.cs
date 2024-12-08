using AOC_2024;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static AOC_2024.Day6Solver;

namespace AOC_2024;


internal class InputDay6(InputGuard guard, int[][] room, List<Tuple<int, int>> minePositions)
{
    public InputGuard Guard { get; } = guard;
    public int[][] Room { get; } = room;
    public List<Tuple<int, int>> MinePositions { get; } = minePositions;

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
        List<Tuple<int, int>> minePositions = new();
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
                    minePositions.Add(new(i, j));
                }
                else
                {
                    guard = new InputGuard(i, j, characterToDirection[character]);
                }
            }
        }
        return new InputDay6(guard, room, minePositions);
    }

    internal InputDay6 CreateWithAddedMine(Position candidateMine)
    {
        int posX = candidateMine.PosX;
        int posY = candidateMine.PosY;

        int[][] copiedRoom = Room.Select(row => row.ToArray()).ToArray();
        copiedRoom[posX][posY] = 1;
        return new InputDay6(guard, copiedRoom, minePositions);
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

    internal HashSet<Position> MoveUntilObstacleOrEndOfScreen(int[][] room)
    {
        HashSet<Position> positions = [];
        int[] move = directionToMove[Direction];
        while (NextIsOnScreen(room) && room[PosX + move[0]][PosY + move[1]] == 0)
        {
            PosX = PosX + move[0];
            PosY = PosY + move[1];

            positions.Add(new(PosX, PosY));
        }
        return positions;
    }

    internal void Rotate()
    {
        this.Direction = (Direction + 1) % 4;
    }

    internal bool CyclesIn(int[][] room)
    {
        bool touchedEdgeOfScreen = false;
        HashSet<Position> allWalkedPositions = new();
        HashSet<Tuple<int, Position>> directedObstacleHits = new();
        while (!touchedEdgeOfScreen)
        {
            var newPositions = MoveUntilObstacleOrEndOfScreen(room);
            foreach (var pos in newPositions)
            {
                allWalkedPositions.Add(pos);
            }
            touchedEdgeOfScreen = !NextIsOnScreen(room);
            if (NextIsOnScreen(room) && !NextIsFree(room))
            {
                int direction = Direction;
                Position obstaclePosition = Next();
                var newHit = new Tuple<int, Position>(direction, obstaclePosition);

                if (!directedObstacleHits.Add(newHit))
                {
                    // if we hit the same obstacle with the same direction we're in a loop
                    return true;
                }
            }
            Rotate();
        }
        return false;
    }

    private Position Next()
    {
        int[] move = directionToMove[Direction];
        return new Position(PosX + move[0], PosY + move[1]);
    }
}

internal class Position(int posX, int posY)
{
    public int PosX { get; } = posX;
    public int PosY { get; } = posY;

    internal bool CanBeAdjacentInCycle(Position otherMine)
    {
        // there needs to be a direction in which they`re one square away from each other
        if (Math.Abs(PosX - otherMine.PosX) == 1)
        {
            // with the correct rotation wrt the "always turn right" rule
            return Math.Sign(PosX - otherMine.PosX) == Math.Sign(PosY - otherMine.PosY);
        }
        else if (Math.Abs(PosY - otherMine.PosY) == 1)
        {
            return Math.Sign(PosX - otherMine.PosX) != Math.Sign(PosY - otherMine.PosY);
        }
        return false;
    }

    internal static Position CloseCycle(Position secondToLast, Position last, Position first)
    {
        int newMineX;
        int newMineY;
        // we need to insert the mine between last and first such that it closes the cycle
        // we know second-to-last -> last has a line/col slack of exactly one
        // the 90 degrees rotation from there gives us halflines the new mine is on
        if (Math.Abs(secondToLast.PosX - last.PosX) == 1)
        {
            int oneSlack = secondToLast.PosX - last.PosX;
            newMineY = last.PosY + oneSlack;
            // its X needs to be such that bumping into it sends us to first
            newMineX = first.PosX - oneSlack;
        }
        else if (Math.Abs(secondToLast.PosY - last.PosY) == 1)
        {
            int oneSlack = secondToLast.PosY - last.PosY;
            newMineX = last.PosX - oneSlack;
            newMineY = first.PosY - oneSlack;
        }
        else
        {
            throw new Exception("Should not happen");
        }
        return new Position(newMineX, newMineY);
    }

    public override string ToString()
    {
        return $"({PosX}, {PosY})";
    }

    public override bool Equals(object obj)
    {
        if (obj is Position otherPos)
        {
            return otherPos.PosX == this.PosX && otherPos.PosY == this.PosY;
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + this.PosX.GetHashCode();
        hash = hash * 31 + this.PosY.GetHashCode();
        return hash;
    }
}

internal class Day6Solver
{
    internal int SolvePart1(InputDay6 input)
    {
        Guard guard = input.Guard.Create();
        int[][] room = input.Room;

        HashSet<Position> distinctPositions = [new(guard.PosX, guard.PosY)];
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

    internal int SolvePart2Lame(InputDay6 input)
    {
        HashSet<Position> actualCyclingMines = new();
        int[][] baseRoom = input.Room;
        InputGuard inputGuard = input.Guard;
        for (int i = 0; i < input.Room.Length; i++)
        {
            for (int j = 0; j < input.Room.Length; j++)
            {
                if (baseRoom[i][j] != 1 && !(i == inputGuard.PosX && j == inputGuard.PosY))
                {
                    Position candidateMine = new(i, j);
                    InputDay6 temperedInput = input.CreateWithAddedMine(candidateMine);
                    Guard guard = temperedInput.Guard.Create();
                    int[][] modifiedRoom = temperedInput.Room;
                    if (guard.CyclesIn(modifiedRoom))
                    {
                        actualCyclingMines.Add(candidateMine);
                    }
                }
            }
            // Console.WriteLine($"Done testing floor index {i}");
        }
        return actualCyclingMines.Count();
    }

    internal int SolvePart2(InputDay6 input)
    {
        List<Position> minePositions = input.MinePositions
            .Select(posTuple => new Position(posTuple.Item1, posTuple.Item2))
            .ToList();


        bool[][] mineToCompatibleMineAfter = new bool[minePositions.Count][];
        for (int i = 0; i < mineToCompatibleMineAfter.Length; i++)
        {
            mineToCompatibleMineAfter[i] = new bool[minePositions.Count];
        }
        for (int i = 0; i < minePositions.Count; i++)
        {
            var mine = minePositions[i];
            for (int j = i + 1; j < minePositions.Count; j++)
            {
                var otherMine = minePositions[j];
                if (mine.CanBeAdjacentInCycle(otherMine))
                {
                    mineToCompatibleMineAfter[i][j] = true;
                    mineToCompatibleMineAfter[j][i] = true;
                }
            }
        }

        List<List<int>> almostCycles = FindAllAlmostCycles(mineToCompatibleMineAfter);
        HashSet<Position> candidateMines = new();
        foreach (var completableCycle in almostCycles)
        {
            int secondToLastId = completableCycle[completableCycle.Count - 2];
            int lastId = completableCycle[completableCycle.Count - 1];
            int firstId = completableCycle[0];

            Position first = minePositions[firstId];
            Position secondToLast = minePositions[secondToLastId];
            Position last = minePositions[lastId];

            Position fourth = Position.CloseCycle(secondToLast, last, first);
            candidateMines.Add(fourth);
        }
        List<Position> actualCyclingMines = new();
        foreach (var candidateMine in candidateMines)
        {
            InputDay6 temperedInput = input.CreateWithAddedMine(candidateMine);
            Guard guard = temperedInput.Guard.Create();
            int[][] room = temperedInput.Room;
            if (guard.CyclesIn(room))
            {
                actualCyclingMines.Add(candidateMine);
                // Console.WriteLine($"Found new good mine {candidateMine}");
            }
        }
        return actualCyclingMines.Count();
    }

    List<List<int>> FindAllAlmostCycles(bool[][] incidence)
    {
        var result = new List<List<int>>();
        int size = incidence.Length;
        // guard rotates 90 degrees on each obstacle, so a cycle must be a multiple of 4
        void Backtrack(List<int> currentChain)
        {
            // If the chain is one less than a multiple of 4, add it to results
            if (currentChain.Count > 1 && (currentChain.Count + 1) % 4 == 0)
            {
                result.Add(new List<int>(currentChain));
            }

            // Try to extend the chain
            int last = currentChain[currentChain.Count - 1];
            for (int next = 0; next < size; next++)
            {
                if (incidence[last][next] && !currentChain.Contains(next)) // Avoid cycles
                {
                    currentChain.Add(next);
                    Backtrack(currentChain);
                    currentChain.RemoveAt(currentChain.Count - 1); // Backtrack
                }
            }
        }

        for (int start = 0; start < size; start++)
        {
            Backtrack(new List<int> { start });
        }

        return result;
    }
}

