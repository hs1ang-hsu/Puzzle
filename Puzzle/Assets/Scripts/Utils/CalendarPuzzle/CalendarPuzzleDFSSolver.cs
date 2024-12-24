using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public static class CalendarPuzzleDFSSolver
{
    private static bool solved = false;
    private static List<PolyominoPuzzle> puzzles;
    private static List<int> chess_state;

    private static bool IsValidMove(int[,] state, int pos_row, int pos_col, Puzzle puzzle)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;

                int row = pos_row + i, col = pos_col + j;
                if (row >= m || col >= n || state[row, col] != (int)BoardState.empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsChessSolvable(int black, int white, int chess_state)
    {
        int diff = Mathf.Abs(black - white);
        if (chess_state < diff) return false;
        if ((chess_state & 1) != (diff & 1)) return false;
        return true;
    }

    private static void GetNextState(int[,] state, int pos_row, int pos_col, Puzzle puzzle, out int puzzle_black, out int puzzle_white)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        puzzle_black = 0;
        puzzle_white = 0;
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;
                int row = pos_row + i, col = pos_col + j;
                state[row, col] = (int)puzzle.type;

                if (((row ^ col) & 1) == 0) puzzle_black++;
                else puzzle_white++;
            }
        }
    }

    private static void RemovePuzzleFromBoard(int[,] state, int pos_row, int pos_col, Puzzle puzzle)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;
                int row = pos_row + i, col = pos_col + j;
                state[row, col] = (int)BoardState.empty;
            }
        }
    }

    private static IEnumerator Search(int[,] state, int idx, int black, int white)
    {
        if (idx >= puzzles.Count)
        {
            solved = true;
            yield break;
        }
        if (!IsChessSolvable(black, white, chess_state[idx]))
        {
            yield break;
        }
        Puzzle puzzle = puzzles[idx];

        int m = state.GetLength(0), n = state.GetLength(1), row = puzzle.dim.x, col = puzzle.dim.y;
        for (int rotation = 0; rotation < 4; rotation++)
        {
            puzzle.Rotate(true);
            for (int flip = 0; flip < 2; flip++)
            {
                puzzle.Flip(true);
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (IsValidMove(state, i, j, puzzle))
                        {
                            GetNextState(state, i, j, puzzle, out int puzzle_black, out int puzzle_white);
                            puzzle.position = new Vector2Int(i, j);

                            yield return Search(state, idx + 1, black - puzzle_black, white - puzzle_white) ;
                            if (solved) yield break;
                            RemovePuzzleFromBoard(state, i, j, puzzle);
                        }
                    }
                }
            }
        }
    }

    private static void CountChessState(int[,] state, out int black, out int white)
    {
        // Count chess states of puzzles
        chess_state = new List<int>();
        chess_state.Add(0);
        int sum = 0;
        foreach (var puzzle in puzzles)
        {
            sum += puzzle.GetChessState();
            chess_state.Add(sum);
        }
        for (int i=0; i<chess_state.Count; i++)
        {
            chess_state[i] = sum - chess_state[i];
        }

        // Count chess states of the board
        int m = state.GetLength(0), n = state.GetLength(1);
        black = 0;
        white = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (state[i, j] != (int)BoardState.empty) continue;
                if (((i ^ j) & 1) == 0) black++;
                else white++;
            }
        }
    }

    public static IEnumerator Solve(int[,] initial_state, List<PolyominoPuzzle> input_puzzles, System.Action callback = null)
    {
        // initialize
        solved = false;
        puzzles = input_puzzles;
        List<int> initial_rotations = new List<int>();
        List<bool> initial_flips_x = new List<bool>(), initial_flips_y = new List<bool>();
        foreach (var puzzle in input_puzzles)
        {
            initial_rotations.Add(puzzle.rotation);
            initial_flips_x.Add(puzzle.flip_x);
            initial_flips_y.Add(puzzle.flip_y);
        }
        CountChessState(initial_state, out int black, out int white);

        // solve
        int num = puzzles.Count;
        yield return Search(initial_state, 0, black, white);

        // reset
        if (!solved)
        {
            for (int i=0; i<input_puzzles.Count; i++)
            {
                input_puzzles[i].position = new Vector2Int(-1, -1);
                input_puzzles[i].rotation = initial_rotations[i];
                input_puzzles[i].flip_x = initial_flips_x[i];
                input_puzzles[i].flip_y = initial_flips_y[i];
            }
        }

        callback?.Invoke();
    }
}
