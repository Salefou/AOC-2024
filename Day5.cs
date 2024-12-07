using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        DependencyGraph dependencyGraph = DependencyGraph.From(input);
        foreach (var update in input.Updates)
        {
            if (dependencyGraph.IsFeasible(update))
            {
                int middlePage = update[update.Length / 2];
                totalScore += middlePage;
            }
        }
        return totalScore;
    }

    public int SolvePart2(InputDay5 input)
    {
        int totalScore = 0;
        DependencyGraph dependencyGraph = DependencyGraph.From(input);
        foreach (var update in input.Updates)
        {
            if (!dependencyGraph.IsFeasible(update))
            {
                int[] feasibleUpdate = dependencyGraph.CreateFeasible(update);
                int middlePage = feasibleUpdate[update.Length / 2];
                totalScore += middlePage;
            }
        }
        return totalScore;
    }

    internal class DependencyGraph(Dictionary<int, HashSet<int>> pageToEnforcedBefore)
    {
        public Dictionary<int, HashSet<int>> PageToEnforcedBefore { get; } = pageToEnforcedBefore;

        internal static DependencyGraph From(InputDay5 input)
        {
            Dictionary<int, HashSet<int>> pageToEnforcedBefore = BuildEnforcedBefore(input);
            return new DependencyGraph(pageToEnforcedBefore);
        }

        private static Dictionary<int, HashSet<int>> BuildEnforcedBefore(InputDay5 input)
        {
            Dictionary<int, HashSet<int>> pageToDisallowedBefore = new();
            foreach (var rule in input.Rules)
            {
                var enforcedBefore = rule.Right;
                if (pageToDisallowedBefore.TryGetValue(enforcedBefore, out HashSet<int> value))
                {
                    value.Add(rule.Left);
                }
                else
                {
                    pageToDisallowedBefore[enforcedBefore] = [rule.Left];
                }
            }
            return pageToDisallowedBefore;
        }

        internal int[] CreateFeasible(int[] update)
        {
            Dictionary<int, HashSet<int>> updatePredecessors = update.ToDictionary(
                page => page,
                page => PageToEnforcedBefore.ContainsKey(page)
                ? PageToEnforcedBefore[page].Intersect(update).ToHashSet()
                : []
            );
            List<int> fixedUpdate = update.OrderBy(page => updatePredecessors[page].Count).ToList();
            bool changedSomeThing = true;
            while (changedSomeThing)
            {
                HashSet<int> alreadyMet = new();
                changedSomeThing = false;
                for (int i = 0; i < fixedUpdate.Count; i++)
                {
                    int page = fixedUpdate[i];
                    HashSet<int> missingBefore = updatePredecessors[page];
                    missingBefore.RemoveWhere(alreadyMet.Contains);
                    alreadyMet.Add(page);
                    if (missingBefore.Count > 0)
                    {
                        changedSomeThing = true;
                        // we're removing those from later down the list so the index for i is still correct for insertion
                        // and we break right after, so no self concurrent modification
                        fixedUpdate.RemoveAll(missingBefore.Contains);
                        foreach (var missingPage in missingBefore)
                        {
                            fixedUpdate.Insert(i, missingPage);
                        }
                        break;
                    }
                }
            }
            return [.. fixedUpdate];
        }

        internal bool IsFeasible(int[] update)
        {
            HashSet<int> runningDisallowedAfter = new();
            foreach (int page in update)
            {
                if (runningDisallowedAfter.Contains(page))
                {
                    return false;
                }
                else if (PageToEnforcedBefore.TryGetValue(page, out HashSet<int> newDisallowedPages))
                {
                    foreach (var newDisallowedPage in newDisallowedPages)
                    {
                        runningDisallowedAfter.Add(newDisallowedPage);
                    }
                }
            }
            return true;
        }
    }
}
