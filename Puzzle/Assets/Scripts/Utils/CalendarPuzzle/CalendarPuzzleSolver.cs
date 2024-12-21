using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalendarPuzzleSolver
{
    public static IEnumerator Solve(int[,] initial_state, List<PuzzleType> puzzle_types)
    {
        int num = puzzle_types.Count;
        List<PuzzleProperty> puzzles = new List<PuzzleProperty>();
        for (int i = 0; i < num; i++)
        {
            puzzles.Add(PuzzleProperties.GetProperty(puzzle_types[i]));
        }
        yield break;
    }
}
