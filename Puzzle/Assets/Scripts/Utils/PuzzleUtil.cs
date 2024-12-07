using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleType
{
    C,
    O,
    T,
    Z,
    V,
    L,
    S,
    B
}

public class PuzzleUtil : MonoBehaviour
{
    public static PuzzleUtil instance;

    // instances
    private GridUtil grid_util;
    private GameManager GM;

    // puzzle
    public float z_depth = 10f;
    public List<GameObject> obj_puzzles;
    private LRU<GameObject> puzzle_order;
    public Transform puzzle_parent;
    private bool initialized;
    private Vector2[] initial_pos;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize parameters
        initialized = false;

        // class instance
        grid_util = GridUtil.instance;
        GM = GameManager.instance;

        // initialize puzzles
        if (obj_puzzles != null)
        {
            puzzle_order = new LRU<GameObject>(obj_puzzles.Count);
            foreach (var obj_puzzle in obj_puzzles)
            {
                puzzle_order.Put(obj_puzzle);
            }
        }
        if (GM.editor_mode)
        {
            obj_puzzles = new List<GameObject>();
            initial_pos = new Vector2[8] {
                new Vector2(0f, 4.5f), new Vector2(3.5f, 4.5f), new Vector2(7f, 4.5f),
                new Vector2(0f, 1f), new Vector2(3.5f, 1f), new Vector2(7f, 1f),
                new Vector2(0f, -2.5f), new Vector2(4.5f, -2.5f)
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.editor_mode && grid_util.initialized && !initialized)
        {
            initialized = true;
            int idx = 0;
            puzzle_order = new LRU<GameObject>(Enum.GetNames(typeof(PuzzleType)).Length);
            foreach (PuzzleType t in Enum.GetValues(typeof(PuzzleType)))
            {
                StartCoroutine(InitializePuzzle(t, initial_pos[idx]));
                idx++;
            }
            InitializePuzzlePos();
        }
    }

    private IEnumerator InitializePuzzle(PuzzleType type, Vector2 pos)
    {
        // Create GameObject
        GameObject obj_puzzle = new GameObject();
        obj_puzzle.tag = "puzzle";
        obj_puzzle.transform.parent = puzzle_parent;

        // Component Puzzle
        Puzzle puzzle = obj_puzzle.AddComponent<Puzzle>();
        puzzle.Initialize(type, z_depth);
        obj_puzzle.name = puzzle.obj_name;

        // Component collider
        Rigidbody2D rigid_body = obj_puzzle.AddComponent<Rigidbody2D>();
        rigid_body.constraints = RigidbodyConstraints2D.FreezeAll;
        CompositeCollider2D collider = obj_puzzle.AddComponent<CompositeCollider2D>();
        collider.geometryType = CompositeCollider2D.GeometryType.Polygons;

        // Draw grids
        puzzle.children = grid_util.GetPuzzleGrids(puzzle, obj_puzzle.transform, 0f);
        UpdatePuzzle(puzzle);
        obj_puzzles.Add(obj_puzzle);
        puzzle_order.Put(obj_puzzle);

        yield break;
    }

    public void InitializePuzzlePos()
    {
        for (int i=0; i<obj_puzzles.Count; i++)
        {
            obj_puzzles[i].transform.position = grid_util.GetPositionByOrigin(initial_pos[i].x, initial_pos[i].y, z_depth);
        }
    }

    public void SortPuzzle(GameObject puzzle)
    {
        puzzle_order.Update(puzzle);
        GameObject[] sorted_puzzles = puzzle_order.GetList();
        float z = z_depth;
        for (int i=0; i<sorted_puzzles.Length; i++)
        {
            sorted_puzzles[i].GetComponent<Puzzle>().UpdateDepth(z);
            z += 1f;
        }
    }

    private void UpdatePuzzle(Puzzle puzzle)
    {
        int n = puzzle.size;
        for (int i=0; i<n; i++)
        {
            for (int j=0; j<n; j++)
            {
                if (puzzle.shape[i,j] == 0)
                {
                    puzzle.children[i][j].SetActive(false);
                } else
                {
                    puzzle.children[i][j].SetActive(true);
                }
            }
        }
    }
}
