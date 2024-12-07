using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UI.Image;

public class GridUtil : MonoBehaviour
{
    public static GridUtil instance;

    // device properties
    private float screen_width;
    private float screen_height;

    // grid properties
    [HideInInspector]
    public bool initialized = false;
    public int grid_num = 7;
    private Vector3 UL, UR, BL, BR;
    private float L, R, B, U;
    private float board_width;
    private float grid_width;
    private float grid_width_scale;
    private List<List<Vector3>> grids_pos;
    private float z_depth = 20f;

    // gameObject
    private ObjectPooler object_pooler;

    // camera
    float ratio;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize parameters
        screen_width = (float)Screen.width;
        screen_height = (float)Screen.height;
        object_pooler = ObjectPooler.instance;
        Vector3 coord_ratio = Camera.main.ScreenToWorldPoint(new Vector3((float)screen_width / 2f + 1f, 0f, 0f));
        ratio = coord_ratio.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitializeGrids(int grid_num)
    {
        this.grid_num = grid_num;
        // Initialize grid
        grids_pos = new List<List<Vector3>>(grid_num);
        for (int i = 0; i < grid_num; i++)
        {
            grids_pos.Add(new List<Vector3>(grid_num));
            for (int j = 0; j < grid_num; j++)
            {
                grids_pos[i].Add(Vector3.zero);
            }
        }

        board_width = screen_height * 0.55f;
        grid_width = board_width / (float)grid_num;
        L = 0.08f * screen_width;
        B = ((float)screen_height - board_width) * 0.6f;
        R = L + board_width;
        U = B + board_width;
        UL = new Vector3(L, U, 15);
        UR = new Vector3(R, U, 15);
        BL = new Vector3(L, B, 15);
        BR = new Vector3(R, B, 15);

        // Initialize grid positions
        InitiailizeGridsPos();
    }

    private void InitiailizeGridsPos()
    {
        float dt = 1 / (float)grid_num;
        for (int t = 0; t < grid_num; t++)
        {
            Vector3 row = Vector3.Lerp(UL, BL, dt * t), col = Vector3.Lerp(BL, BR, dt * t);
            for (int i = 0; i < grid_num; i++)
            {
                grids_pos[t][i] = new Vector3(grids_pos[t][i].x, row.y, 0f);
                grids_pos[i][t] = new Vector3(col.x, grids_pos[i][t].y, 0f);
            }
        }

        grid_width_scale = ScreenToWorld(grid_width) / 5.12f;
        for (int i = 0; i < grid_num; i++)
            for (int j = 0; j < grid_num; j++)
            {
                grids_pos[i][j] = Camera.main.ScreenToWorldPoint(grids_pos[i][j]);
                grids_pos[i][j] = new Vector3(grids_pos[i][j].x, grids_pos[i][j].y, z_depth);
            }
        initialized = true;
    }

    private void DebugDrawBoard()
    {
        float dt = 1 / (float)grid_num;
        for (int t = 0; t < grid_num; t++)
        {
            Vector3 row = Vector3.Lerp(UL, BL, dt * t), col = Vector3.Lerp(BL, BR, dt * t);
            Debug.DrawLine(Camera.main.ScreenToWorldPoint(row), Camera.main.ScreenToWorldPoint(row + new Vector3(board_width, 0, 0)));
            Debug.DrawLine(Camera.main.ScreenToWorldPoint(col), Camera.main.ScreenToWorldPoint(col + new Vector3(0, board_width, 0)));
        }
    }

    public Vector3 GetPositionByOrigin(float x, float y, float z=-1)
    {
        float origin_x = R + 40f, origin_y = (float)screen_height / 2;
        Vector3 result = Camera.main.ScreenToWorldPoint(new Vector3(origin_x + grid_width * x, origin_y + grid_width * y, 0f));
        result.z = (z < 0) ? z_depth : z;
        return result;
    }

    public Vector3 GetPosition(int row, int col, Vector3 point, float z=-1)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");

        if (row < 0 || row >= grid_num || col < 0 || col >= grid_num) return point;
        Vector3 result = grids_pos[row][col];
        result.z = (z < 0) ? z_depth : z;
        return result;
    }

    public Vector3 ToGridPoint(float mx, float my, out int grid_row, out int grid_col, Vector3 point, float z)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");

        if (mx < L - grid_width/2 || mx > R - grid_width/2 || my < B + grid_width/2 || my > U + grid_width/2)
        {
            grid_row = -1;
            grid_col = -1;
            return point;
        }

        // Note that in grids, x means row and y means col
        mx = Mathf.Clamp(mx, L, R - grid_width);
        my = Mathf.Clamp(my, B + grid_width, U);
        grid_col = (int)Mathf.Round((mx - L) / grid_width);
        grid_row = (int)Mathf.Round((U - my) / grid_width);
        float x = L + grid_width * grid_col;
        float y = U - grid_width * grid_row;
        Vector3 result = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0f));
        result.z = (z < 0) ? z_depth : z;
        return result;
    }

    private float ScreenToWorld(float d)
    {
        return d * ratio;
    }

    public List<List<GameObject>> GetBoardGrids(int[,] shape, Transform grids_parent, float z)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");

        List<List<GameObject>> obj_grids = new List<List<GameObject>>();
        for (int i = 0; i < grid_num; i++)
        {
            obj_grids.Add(new List<GameObject>());
            for (int j = 0; j < grid_num; j++)
            {
                if (shape[i,j] == 0)
                {
                    obj_grids[i].Add(null);
                    continue;
                }

                GameObject new_obj_grid = object_pooler.SpawnFromPool("grid", grids_pos[i][j], Quaternion.identity, grids_parent);
                new_obj_grid.transform.localScale = new Vector3(grid_width_scale, grid_width_scale, 1f);
                new_obj_grid.transform.position = new Vector3(new_obj_grid.transform.position.x, new_obj_grid.transform.position.y, z);
                new_obj_grid.name = "Grid#" + i.ToString() + "_" + j.ToString();
                obj_grids[i].Add(new_obj_grid);
            }
        }
        return obj_grids;
    }

    public List<List<GameObject>> GetPuzzleGrids(Puzzle puzzle, Transform grids_parent, float z)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");
        int n = puzzle.size;
        if (n >= grid_num) Debug.LogError("The puzzle is larger than the board!");

        Vector3 reference = grids_pos[0][0];
        List<List<GameObject>> obj_puzzle_grids = new List<List<GameObject>>();
        for (int i = 0; i < n; i++)
        {
            obj_puzzle_grids.Add(new List<GameObject>());
            for (int j = 0; j < n; j++)
            {
                GameObject new_obj_grid = object_pooler.SpawnFromPool("grid", grids_pos[i][j] - reference, Quaternion.identity, grids_parent);
                new_obj_grid.transform.localScale = new Vector3(grid_width_scale, grid_width_scale, 1f);
                new_obj_grid.transform.position = new Vector3(new_obj_grid.transform.position.x, new_obj_grid.transform.position.y, z);
                new_obj_grid.name = puzzle.obj_name + "#" + i.ToString() + "_" + j.ToString();
                new_obj_grid.GetComponent<SpriteRenderer>().color = puzzle.color;
                obj_puzzle_grids[i].Add(new_obj_grid);
            }
        }
        return obj_puzzle_grids;
    }
}
