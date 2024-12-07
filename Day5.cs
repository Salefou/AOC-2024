using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC_2024;

internal class InputDay5(InputRule[] rules, int[][] updates)
{
    public InputRule[] Rules { get; } = rules;
    public int[][] Updates { get; } = updates;

    internal static InputDay5 Parse(string pathToFile)
    {
        string[] lines = File.ReadAllLines(pathToFile);

        bool parsingRules = true;
        List<InputRule> rules = [];
        List<int[]> updates = [];
        foreach (var line in lines)
        {
            if (line == "")
            {
                parsingRules = false;
            }
            else if (parsingRules)
            {
                int[] leftAndRight = line.Split('|').Select(int.Parse).ToArray();
                rules.Add(new InputRule(leftAndRight[0], leftAndRight[1]));
            }
            else
            {
                int[] update = line.Split(',').Select(int.Parse).ToArray();
                updates.Add(update);
            }
        }
        return new InputDay5([.. rules], [.. updates]);
    }
}


internal class InputRule(int left, int right)
{
    public int Left { get; } = left;
    public int Right { get; } = right;
}

internal class Day5Solver
{
    internal int SolvePart1(InputDay5 input)
    {
        int totalScore = 0;
        Dictionary<int, HashSet<int>> pageToDisallowedAfter = PageToDisallowedAfter(input);
        foreach (var update in input.Updates)
        {
            bool isFeasibleUpdate = true;
            HashSet<int> runningDisallowedAfter = new();
            foreach (int page in update)
            {
                if (runningDisallowedAfter.Contains(page))
                {
                    isFeasibleUpdate = false;
                    break;
                }
                else
                {
                    if (pageToDisallowedAfter.TryGetValue(page, out HashSet<int> newDisallowedPages))
                    {
                        foreach (var newDisallowedPage in newDisallowedPages)
                        {
                            runningDisallowedAfter.Add(newDisallowedPage);
                        }
                    }
                }
            }
            if (isFeasibleUpdate)
            {
                int middleUpdate = update[update.Length / 2];
                totalScore += middleUpdate;
            }
        }
        return totalScore;
    }

    private Dictionary<int, HashSet<int>> PageToDisallowedAfter(InputDay5 input)
    {
        Dictionary<int, HashSet<int>> pageToDisallowedAfter = new();
        foreach (var rule in input.Rules)
        {
            var enforcedAfter = rule.Right;
            if (pageToDisallowedAfter.TryGetValue(enforcedAfter, out HashSet<int> value))
            {
                value.Add(rule.Left);
            }
            else
            {
                pageToDisallowedAfter[enforcedAfter] = [rule.Left];
            }
        }
        return pageToDisallowedAfter;
    }
}
