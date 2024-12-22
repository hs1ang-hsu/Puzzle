using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    none = -5,
    removed = -2,
    empty = -1
}

public class Board : MonoBehaviour
{
    // instance
    public GridUtil grid_util;
    public PuzzleUtil puzzle_util;
    public GameManager game_manager;

    // parameters
    [HideInInspector] public Vector2Int board_dim;
    [HideInInspector] public int removed_limit;
    [HideInInspector] public float z_depth;
    [HideInInspector] public int[,] grids; // none: -5, removed: -2, empty: -1
    private LRU<Vector2Int> removed_grids;
    [HideInInspector] public bool initialized;
    [HideInInspector] public int[,] shape;

    // GameObject
    public List<List<GameObject>> obj_grids;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator InitializeBoard(int[,] shape, Vector2Int board_dim, int removed_limit, float z_depth)
    {
        // initialize parameters
        this.shape = shape;
        this.board_dim = board_dim;
        this.removed_limit = removed_limit;
        this.z_depth = z_depth;

        // generate grids
        grid_util.InitializeGrids(board_dim);
        removed_grids = new LRU<Vector2Int>(removed_limit, new Vector2Int(-1,-1));
        grids = new int[board_dim.x, board_dim.y];
        for (int i = 0; i < board_dim.x; i++)
            for (int j = 0; j < board_dim.y; j++)
                grids[i, j] = (shape[i,j] == 1) ? (int)BoardState.empty : (int)BoardState.none;

        obj_grids = grid_util.GetBoardGrids(shape, transform, z_depth);
        initialized = true;
        yield break;
    }

    public void ResetBoard()
    {
        removed_grids.Clear();
        for (int i = 0; i < board_dim.x; i++)
        {
            for (int j = 0; j < board_dim.y; j++)
            {
                if (grids[i,j] != (int)BoardState.none)
                {
                    RecoverGrid(i, j);
                }
            }
        }
    }

    public void EditBoard(Transform obj_grid)
    {
        string name = obj_grid.name.Split('#')[1];
        int x = int.Parse(name.Split('_')[0]), y = int.Parse(name.Split('_')[1]);

        if (grids[x,y] == (int)BoardState.removed)
        {
            removed_grids.Remove(new Vector2Int(x, y));
            RecoverGrid(x, y);
            return;
        }

        Vector2Int recover = removed_grids.Put(new Vector2Int(x, y));
        RemoveGrid(x, y);
        if (recover.x != -1)
        {
            RecoverGrid(recover.x, recover.y);
        }
    }

    public void Selected()
    {
        for (int i = 0; i < board_dim.x; i++)
        {
            for (int j = 0; j < board_dim.y; j++)
            {
                if (grids[i,j] != (int)BoardState.none)
                {
                    obj_grids[i][j].transform.Find("Highlight").gameObject.SetActive(true);
                }
            }
        }
    }

    public void Unselected()
    {
        for (int i = 0; i < board_dim.x; i++)
        {
            for (int j = 0; j < board_dim.y; j++)
            {
                if (grids[i,j] != (int)BoardState.none)
                {
                    obj_grids[i][j].transform.Find("Highlight").gameObject.SetActive(false);
                }
            }
        }
    }

    private void RemoveGrid(int x, int y)
    {
        grids[x,y] = (int)BoardState.removed;
        obj_grids[x][y].GetComponent<SpriteRenderer>().color = Color.black;
    }

    private void RecoverGrid(int x, int y)
    {
        grids[x,y] = (int)BoardState.empty;
        obj_grids[x][y].GetComponent<SpriteRenderer>().color = Color.white;
    }

    public int[,] GetPolyominoPuzzleBoardState(out bool is_board_valid, out List<Puzzle> unused_puzzles)
    {
        // load current grid state
        int[,] result = new int[board_dim.x, board_dim.y];
        for (int i = 0; i < board_dim.x; i++)
            for (int j = 0; j < board_dim.y; j++)
                result[i, j] = grids[i, j];

        // load puzzles on the board
        is_board_valid = true;
        unused_puzzles = new List<Puzzle>();
        foreach (var obj_puzzle in puzzle_util.obj_puzzles)
        {
            Puzzle puzzle = obj_puzzle.GetComponent<Puzzle>();
            Vector2Int position = puzzle.position;
            if (position.x < 0)
            {
                unused_puzzles.Add(puzzle);
                continue;
            }

            int[,] puzzle_shape = puzzle.shape;
            int puzzle_size = puzzle.size;
            for (int i=0; i<puzzle_size; i++)
            {
                for (int j=0; j<puzzle_size; j++)
                {
                    if (puzzle_shape[i, j] == 0) continue;

                    int row = position.x + i, col = position.y + j;
                    if (row >= board_dim.x || col >= board_dim.y || result[row, col] != (int)BoardState.empty)
                    {
                        is_board_valid = false;
                        return result;
                    }
                    result[row, col] = (int)(puzzle.type);
                }
            }
        }

        return result;
    }
}
