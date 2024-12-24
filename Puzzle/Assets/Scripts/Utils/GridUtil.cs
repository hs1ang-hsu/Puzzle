using System.Collections.Generic;
using UnityEngine;

public class GridUtil : MonoBehaviour
{
    // device properties
    private float screen_width;
    private float screen_height;

    // grid properties
    [HideInInspector] public bool initialized = false;
    [HideInInspector] public Vector2Int board_dim;
    private Vector3 UL, UR, BL, BR;
    private float L, R, B, U;
    private float board_width;
    private float board_height;
    public float image_size = 2.56f;
    [HideInInspector] public float grid_width = 64;
    [HideInInspector] public float grid_width_scale;
    private List<List<Vector3>> grids_pos;
    private float z_depth = 20f;

    // gameObject
    public ObjectPooler object_pooler;

    // camera
    float ratio;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize parameters
        screen_width = (float)Screen.width;
        screen_height = (float)Screen.height;
        Vector3 coord_ratio = Camera.main.ScreenToWorldPoint(new Vector3((float)screen_width / 2f + 1f, 0f, 0f));
        ratio = coord_ratio.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitializeGrids(Vector2Int board_dim)
    {
        this.board_dim = board_dim;
        // Initialize grid
        grids_pos = new List<List<Vector3>>(board_dim.x);
        for (int i = 0; i < board_dim.x; i++)
        {
            grids_pos.Add(new List<Vector3>(board_dim.y));
            for (int j = 0; j < board_dim.y; j++)
            {
                grids_pos[i].Add(Vector3.zero);
            }
        }

        board_height = screen_height * 0.55f;
        grid_width = board_height / (float)board_dim.x;
        board_width = board_dim.y * grid_width;

        L = 0.08f * screen_width;
        B = ((float)screen_height - board_height) * 0.6f;
        R = L + board_width;
        U = B + board_height;
        UL = new Vector3(L, U, 15);
        UR = new Vector3(R, U, 15);
        BL = new Vector3(L, B, 15);
        BR = new Vector3(R, B, 15);

        // Initialize grid positions
        InitiailizeGridsPos();
    }

    private void InitiailizeGridsPos()
    {
        float dx = 1 / (float)board_dim.x, dy = 1 / (float)board_dim.y;
        for (int x = 0; x < board_dim.x; x++)
        {
            Vector3 row = Vector3.Lerp(UL, BL, dx * x);
            for (int y = 0; y < board_dim.y; y++)
            {
                Vector3 col = Vector3.Lerp(BL, BR, dy * y);
                grids_pos[x][y] = new Vector3(col.x, row.y, 0f);
            }
        }

        grid_width_scale = ScreenToWorld(grid_width) / image_size;
        for (int i = 0; i < board_dim.x; i++)
            for (int j = 0; j < board_dim.y; j++)
            {
                grids_pos[i][j] = Camera.main.ScreenToWorldPoint(grids_pos[i][j]);
                grids_pos[i][j] = new Vector3(grids_pos[i][j].x, grids_pos[i][j].y, z_depth);
            }
        initialized = true;
    }

    public Vector3 GetPositionByOrigin(float x, float y, float z=-1)
    {
        float origin_x = R + 40f, origin_y = (float)screen_height / 2;
        Vector3 result = Camera.main.ScreenToWorldPoint(new Vector3(origin_x + grid_width * x, origin_y + grid_width * y, 0f));
        result.z = (z < 0) ? z_depth : z;
        return result;
    }

    public Vector3 GetPosition(int row, int col, Vector3 point, float z=-1, float shift_x = 0f, float shift_y = 0f)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");

        if (row < 0 || row >= board_dim.x || col < 0 || col >= board_dim.y) return point;
        Vector3 result = Camera.main.ScreenToWorldPoint(new Vector3(L + grid_width * col - shift_x, U - grid_width * row - shift_y, 0f));
        result.z = (z < 0) ? z_depth : z;
        return result;
    }

    public Vector3 ToGridPoint(float mx, float my, out int grid_row, out int grid_col, Vector3 point, float z, float shift_x = 0f, float shift_y = 0f)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");

        mx += shift_x;
        my += shift_y;
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
        float x = L + grid_width * grid_col - shift_x;
        float y = U - grid_width * grid_row - shift_y;
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
        for (int i = 0; i < board_dim.x; i++)
        {
            obj_grids.Add(new List<GameObject>());
            for (int j = 0; j < board_dim.y; j++)
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

    public List<List<GameObject>> GetPuzzleGrids(PolyominoPuzzle puzzle, Transform grids_parent, float z)
    {
        if (!initialized) Debug.LogError("The grids are not initialized!");
        int n = puzzle.size;
        if (n >= board_dim.x) Debug.LogError("The puzzle is larger than the board!");

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

    public Vector3 GetGridScale()
    {
        return new Vector3(grid_width_scale, grid_width_scale, 1f);
    }

    public Mesh GetPuzzleMesh(int[,] shape, int size)
    {
        Mesh mesh_puzzle = new Mesh();

        List<int> ref_triangles = new List<int>() { 0,1,3, 0,3,2 };
        List<Vector3> ref_vertices = new List<Vector3>()
        {
            new Vector3(image_size, 0, 0),
            new Vector3(0, -image_size, 0),
            new Vector3(image_size, -image_size, 0)
        };

        int idx_e = 0;
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        for (int i=0; i<size; i++)
        {
            for (int j=0; j<size; j++)
            {
                if (shape[i,j] == 1)
                {
                    Vector3 pos = new Vector3(j * image_size, -i * image_size, 0);
                    vertices.Add(pos);
                    foreach (var ref_v in ref_vertices)
                    {
                        vertices.Add(pos + ref_v);
                    }
                    foreach (var ref_t in ref_triangles)
                    {
                        triangles.Add(idx_e + ref_t);
                    }
                    idx_e += 4;
                }
            }
        }
        mesh_puzzle.vertices = vertices.ToArray();
        mesh_puzzle.triangles = triangles.ToArray();
        return mesh_puzzle;
    }
}
