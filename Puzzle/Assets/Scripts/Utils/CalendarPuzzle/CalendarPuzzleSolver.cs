using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.IK;
using static CalendarPuzzleSolver;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.Rendering.DebugUI.Table;

public class CalendarPuzzleSolver
{
    internal class PuzzleState
    {
        public int rotation;
        public bool flip_x;
        public bool flip_y;
        public Vector2Int position;
        public PuzzleState(int rotation, bool flip_x, bool flip_y, Vector2Int position)
        {
            this.rotation = rotation;
            this.flip_x = flip_x;
            this.flip_y = flip_y;
            this.position = position;
        }

        public PuzzleState()
        {
            rotation = 0;
            flip_x = false;
            flip_y = false;
            position = Vector2Int.zero;
        }
    }

    private List<bool[]> matrix;
    private List<PuzzleState> puzzle_states;
    private int width;
    private int num_puzzle;
    private Dictionary<PuzzleType, int> puzzle_type_indices;
    public List<PolyominoPuzzle> puzzles;
    private int m;
    private int n;

    private bool IsValidMove(int[,] state, int pos_row, int pos_col, Puzzle puzzle)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;

                int row = pos_row + i, col = pos_col + j;
                if (row >= m || col >= n || state[row, col] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void GetNextState(int[,] state, int pos_row, int pos_col, Puzzle puzzle)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;
                int row = pos_row + i, col = pos_col + j;
                state[row, col] = 1;
            }
        }
    }

    private void RemovePuzzleFromBoard(int[,] state, int pos_row, int pos_col, Puzzle puzzle)
    {
        int m = state.GetLength(0), n = state.GetLength(1);
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (puzzle.shape[i, j] == 0) continue;
                int row = pos_row + i, col = pos_col + j;
                state[row, col] = 0;
            }
        }
    }

    private void AddFlattenedState(int[,] state, PuzzleType puzzle_type=PuzzleType.None, int[,] initial_state=null)
    {
        bool[] row = new bool[width];
        for (int i=0; i<m; i++)
        {
            for (int j=0; j<n; j++)
            {
                if (initial_state != null && initial_state[i, j] == 1)
                {
                    row[i * m + j] = false;
                    continue;
                }
                row[i * m + j] = state[i, j] == 1;
            }
        }
        for (int i=0; i<num_puzzle; i++)
        {
            row[m*n + i] = false;
        }
        if (puzzle_type != PuzzleType.None)
            row[puzzle_type_indices[puzzle_type]] = true;

        matrix.Add(row);
    }

    private void GenerateAllStates(int[,] state, PolyominoPuzzle puzzle, int[,] initial_state)
    {
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
                            GetNextState(state, i, j, puzzle);
                            AddFlattenedState(state, puzzle.type, initial_state);
                            puzzle_states.Add(new PuzzleState(puzzle.rotation, puzzle.flip_x, puzzle.flip_y, new Vector2Int(i, j)));
                            RemovePuzzleFromBoard(state, i, j, puzzle);
                        }
                    }
                }
            }
        }
    }

    private void SetupMatrix(int[,] initial_state)
    {
        // process initial state to 0s and 1s
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                initial_state[i, j] = (initial_state[i, j] == (int)BoardState.empty) ? 0 : 1;
        AddFlattenedState(initial_state);
        puzzle_states.Add(new PuzzleState());

        // Copy
        int[,] state = new int[m, n];
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                state[i, j] = initial_state[i, j];
        
        // initialize matrix
        foreach (var puzzle in puzzles)
        {
            GenerateAllStates(state, puzzle, initial_state);
        }
    }

    private int[,] GetPuzzleShape(bool[] row, PolyominoPuzzle puzzle)
    {
        // reconstruct state from row
        bool[,] state = new bool[m, n];
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                state[i, j] = row[i * m + j];
            }
        }

        // reconstruct shape
        Vector2Int position = puzzle.position;
        int[,] shape = new int[puzzle.size, puzzle.size];
        for (int i = 0; i < puzzle.size; i++)
        {
            for (int j = 0; j < puzzle.size; j++)
            {
                if (position.x + i >= m || position.y + j >= n || !state[position.x + i, position.y + j])
                {
                    shape[i, j] = 0;
                } else
                {
                    shape[i, j] = 1;
                }
            }
        }
        return shape;
    }

    private void UpdatePuzzlesWithSolution(List<int> solution)
    {
        foreach (int row in solution)
        {
            if (row == 0) continue;
            
            PuzzleState puzzle_state = puzzle_states[row];

            int idx = 0;
            for (; idx < width; idx++)
            {
                if (matrix[row][m*n + idx]) break;
            }

            puzzles[idx].rotation = puzzle_state.rotation;
            puzzles[idx].flip_x = puzzle_state.flip_x;
            puzzles[idx].flip_y = puzzle_state.flip_y;
            puzzles[idx].position = puzzle_state.position;
            puzzles[idx].shape = GetPuzzleShape(matrix[row], puzzles[idx]);
        }
    }

    private void PrintState(List<bool> state)
    {
        string s = "";
        for (int i = 0; i < state.Count; i++)
        {
            if (i > 0 && i % 7 == 0) s += "\n";
            s += (state[i] ? 1 : 0).ToString();
        }
        Debug.Log(s);
    }

    // Recursive IEnumerator work around: https://gamedev.stackexchange.com/questions/150940/recursive-unity-coroutine
    public IEnumerator Solve(int[,] initial_state, List<PolyominoPuzzle> input_puzzles, System.Action callback = null)
    {
        // initialize
        matrix = new List<bool[]>();
        puzzle_states = new List<PuzzleState>();
        puzzle_type_indices = new Dictionary<PuzzleType, int>();
        m = initial_state.GetLength(0);
        n = initial_state.GetLength(1);
        width = m * n;
        puzzles = input_puzzles;
        num_puzzle = puzzles.Count;
        foreach (var puzzle in input_puzzles)
        {
            puzzle_type_indices[puzzle.type] = width;
            width++;
        }
        
        // setup matrix
        SetupMatrix(initial_state);
        bool[][] matrix_arr = matrix.ToArray();
        yield return null;

        // solve
        DLXLib.DLX solver = new DLXLib.DLX(matrix_arr);
        yield return solver.Search(0);
        Debug.Log("Iterations: " + solver.iteration.ToString());

        // get solution and update puzzles
        List<int> solution = new List<int>();
        solution = solver.CurrentSolution.ToList();
        if (solver.Solved)
        {
            UpdatePuzzlesWithSolution(solution);
        }
        callback?.Invoke();
        yield return null;
    }
}
