using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC_2024;

internal class InputDay2(List<int[]> reportList)
{
    public List<int[]> ReportList { get; } = reportList;

    internal static InputDay2 Parse(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        List<int[]> reportList = lines.Select(line => line.Split().Select(lineSplit => int.Parse(lineSplit)).ToArray()).ToList();
        return new InputDay2(reportList);
    }
}

internal class Day2Solver
{
    private bool IsSafe(int[] report)
    {
        bool mightBeIncreasing = true;
        bool mightBeDecreasing = true;

        int previous = report[0];
        for (int i = 1; i < report.Length; i++)
        {
            int current = report[i];
            int difference = current - previous;
            if (Math.Abs(difference) < 1 || Math.Abs(difference) > 3)
            {
                return false;
            }
            if (difference > 0)
            {
                mightBeDecreasing = false;
            }
            else if (difference < 0)
            {
                mightBeIncreasing = false;
            }
            previous = current;
        }
        return mightBeIncreasing || mightBeDecreasing;
    }

    internal int SolvePart1(InputDay2 inputDay2)
    {
        int safeReportCount = 0;
        foreach (int[] report in inputDay2.ReportList)
        {
            if (IsSafe(report))
            {
                safeReportCount++;
            }
        }
        return safeReportCount;
    }



    internal int SolvePart2(InputDay2 inputDay2)
    {
        int almostSafeReportCount = 0;
        foreach (int[] report in inputDay2.ReportList)
        {
            for (int i = 0; i < report.Length; i++)
            {
                int[] maybeFixedReport = report.Where((level, index) => index != i).ToArray();
                if (IsSafe(maybeFixedReport))
                {
                    almostSafeReportCount++;
                    break;
                }
            }
        }
        return almostSafeReportCount;
    }
}

