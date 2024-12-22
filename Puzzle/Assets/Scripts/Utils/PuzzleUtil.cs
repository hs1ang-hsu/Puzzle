using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class PuzzleUtil : MonoBehaviour
{
    // instances
    public GridUtil grid_util;
    public ObjectPooler object_pooler;

    // sprites
    public SpriteAtlas atlas_puzzles;

    // puzzle
    public float z_depth = 10f;
    public List<GameObject> obj_puzzles;
    private LRU<GameObject> puzzle_order;
    public Transform puzzle_parent;
    private List<Vector2> initial_pos;

    // Start is called before the first frame update
    void Start()
    {
        // initialize puzzles
        if (obj_puzzles != null)
        {
            puzzle_order = new LRU<GameObject>(obj_puzzles.Count);
            foreach (var obj_puzzle in obj_puzzles)
            {
                puzzle_order.Put(obj_puzzle);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void GeneratePuzzles(List<PuzzleType> required_puzzle_types, List<Vector2> initial_pos)
    {
        if (initial_pos.Count != required_puzzle_types.Count) Debug.LogError("The initial positions does not match the number of puzzles ");
        this.initial_pos = initial_pos;
        puzzle_order = new LRU<GameObject>(required_puzzle_types.Count);
        foreach (var puzzle_type in required_puzzle_types)
        {
            StartCoroutine(InitializePuzzle(puzzle_type));
        }
        InitializePuzzlePos();
    }

    private IEnumerator InitializePuzzle(PuzzleType type)
    {
        // Create GameObject
        GameObject obj_puzzle = object_pooler.SpawnFromPool("puzzle", Vector3.zero, Quaternion.identity, puzzle_parent);

        // Component Puzzle
        obj_puzzle.GetComponent<Puzzle>().Initialize(type, z_depth, ref atlas_puzzles);
        obj_puzzle.name = obj_puzzle.GetComponent<Puzzle>().obj_name;

        // Update puzzle to lists
        obj_puzzles.Add(obj_puzzle);
        puzzle_order.Put(obj_puzzle);

        yield break;
    }

    public void InitializePuzzlePos()
    {
        for (int i=0; i<obj_puzzles.Count; i++)
        {
            obj_puzzles[i].transform.position = grid_util.GetPositionByOrigin(initial_pos[i].x, initial_pos[i].y, z_depth);
            obj_puzzles[i].GetComponent<Puzzle>().position = new Vector2Int(-1, -1);
        }
    }

    public void SortPuzzle(GameObject puzzle)
    {
        puzzle_order.Update(puzzle);
        GameObject[] sorted_puzzles = puzzle_order.GetList();
        float z = z_depth;
        for (int i=0; i<sorted_puzzles.Length; i++)
        {
            sorted_puzzles[i].GetComponent<PolyominoPuzzle>().UpdateDepth(z);
            z += 1f;
        }
    }
}
