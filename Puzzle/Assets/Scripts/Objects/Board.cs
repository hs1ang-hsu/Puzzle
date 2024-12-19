using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // instance
    public GridUtil grid_util;
    public GameManager game_manager;

    // parameters
    public int grid_num = 7;
    public int removed_limit = 2;
    public float z_depth = 20f;
    [HideInInspector]
    public List<List<int>> grids; // -5: none, -2: removed, -1: normal, 0~7: puzzle type
    private LRU<Vector2Int> removed_grids;
    private bool initialized;
    private int[,] shape = new int[7, 7] {
        { 1,1,1,1,1,1,0 },
        { 1,1,1,1,1,1,0 },
        { 1,1,1,1,1,1,1 },
        { 1,1,1,1,1,1,1 },
        { 1,1,1,1,1,1,1 },
        { 1,1,1,1,1,1,1 },
        { 1,1,1,0,0,0,0 }
    };

    // GameObject
    public List<List<GameObject>> obj_grids;

    // Start is called before the first frame update
    void Start()
    {
        removed_grids = new LRU<Vector2Int>(removed_limit);
        obj_grids = new List<List<GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            initialized = true;
            grid_util.InitializeGrids(grid_num);
            StartCoroutine(InitializeBoard());
        }

    }

    private IEnumerator InitializeBoard()
    {
        removed_grids = new LRU<Vector2Int>(removed_limit, new Vector2Int(-1,-1));
        grids = new List<List<int>>();
        for (int i = 0; i < grid_num; i++)
        {
            grids.Add(new List<int>());
            for (int j = 0; j < grid_num; j++)
                grids[i].Add(-1);
        }

        grids[0][6] = -5;
        grids[1][6] = -5;
        for (int i = 3; i < grid_num; i++)
            grids[6][i] = -5;

        obj_grids = grid_util.GetBoardGrids(shape, transform, z_depth);
        yield break;
    }

    public void ResetBoard()
    {
        for (int i = 0; i < grid_num; i++)
        {
            for (int j = 0; j < grid_num; j++)
            {
                if (grids[i][j] == -2)
                {
                    RecoverGrid(i, j);
                    removed_grids.Remove(new Vector2Int(i, j));
                }
            }
        }
    }

    public void EditBoard(Transform obj_grid)
    {
        string name = obj_grid.name.Split('#')[1];
        int x = int.Parse(name.Split('_')[0]), y = int.Parse(name.Split('_')[1]);

        if (grids[x][y] == -2)
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
        for (int i = 0; i < grid_num; i++)
        {
            for (int j = 0; j < grid_num; j++)
            {
                if (grids[i][j] != -5)
                {
                    obj_grids[i][j].transform.Find("Highlight").gameObject.SetActive(true);
                }
            }
        }
    }

    public void Unselected()
    {
        for (int i = 0; i < grid_num; i++)
        {
            for (int j = 0; j < grid_num; j++)
            {
                if (grids[i][j] != -5)
                {
                    obj_grids[i][j].transform.Find("Highlight").gameObject.SetActive(false);
                }
            }
        }
    }

    private void RemoveGrid(int x, int y)
    {
        grids[x][y] = -2;
        obj_grids[x][y].GetComponent<SpriteRenderer>().color = Color.black;
    }

    private void RecoverGrid(int x, int y)
    {
        grids[x][y] = -1;
        obj_grids[x][y].GetComponent<SpriteRenderer>().color = Color.white;
    }
}
