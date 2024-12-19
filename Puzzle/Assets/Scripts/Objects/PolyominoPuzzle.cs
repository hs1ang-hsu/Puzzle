using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PolyominoPuzzleType
{
    None,
    Dot1,
    H2,
    V2,
    O4,
    F5,
    I5,
    L5,
    N5,
    P5,
    T5,
    U5,
    V5,
    W5,
    X5,
    Y5,
    Z5,
    O6
}

public class PolyominoPuzzle : MonoBehaviour, ISelectableObject, IPooledObject
{
    // instance
    public GameManager game_manager;
    public GridUtil grid_util;

    // parameters
    [HideInInspector] public int[,] shape;
    [HideInInspector] public int grid_num;
    [HideInInspector] public int size;
    [HideInInspector] public Vector2Int dim;
    [HideInInspector] public int rotation;
    [HideInInspector] public int flip;
    [HideInInspector] public string obj_name;
    [HideInInspector] public Color color;
    [HideInInspector] public PolyominoPuzzleType type;
    [HideInInspector] public Vector2Int position;

    // hidden parameters
    private Vector3 offset = Vector3.zero;
    private float z_depth;
    private SpriteRenderer sprite_renderer;

    public void Initialize(PolyominoPuzzleType type, float z_depth, ref SpriteAtlas atlas_puzzles)
    {
        // initialize puzzle parameters
        this.obj_name = "polyomino_" + type.ToString();
        this.type = type;
        this.z_depth = z_depth;
        switch (type)
        {
            case PolyominoPuzzleType.Dot1:
                shape = new int[1, 1] { { 1 } };
                size = 1;
                dim = new Vector2Int(1, 1);
                grid_num = 1;
                color = MyColorPalette.Ivory;
                break;
            case PolyominoPuzzleType.V2:
                shape = new int[2, 2] { { 1, 0 }, { 1, 0 } };
                size = 2;
                dim = new Vector2Int(2, 1);
                grid_num = 2;
                color = MyColorPalette.DarkOrange;
                break;
            case PolyominoPuzzleType.H2:
                shape = new int[2, 2] { { 1, 0 }, { 1, 0 } };
                size = 2;
                dim = new Vector2Int(1, 2);
                grid_num = 2;
                color = MyColorPalette.CornFlowerBlue;
                break;
            case PolyominoPuzzleType.O4:
                shape = new int[2, 2] { { 1, 1 }, { 1, 1 } };
                size = 2;
                dim = new Vector2Int(2, 2);
                grid_num = 4;
                color = MyColorPalette.Gold;
                break;
            case PolyominoPuzzleType.F5:
                shape = new int[3, 3] { { 0, 1, 1 }, { 1, 1, 0 }, { 0, 1, 0 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.Maroon;
                break;
            case PolyominoPuzzleType.I5:
                shape = new int[5, 5] { { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 } };
                size = 5;
                dim = new Vector2Int(5, 1);
                grid_num = 5;
                color = MyColorPalette.OrangeRed;
                break;
            case PolyominoPuzzleType.L5:
                shape = new int[4, 4] { { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 1, 0, 0 } };
                size = 4;
                dim = new Vector2Int(4, 2);
                grid_num = 5;
                color = MyColorPalette.LightCoral;
                break;
            case PolyominoPuzzleType.N5:
                shape = new int[4, 4] { { 0, 0, 1, 1 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
                size = 4;
                dim = new Vector2Int(2, 4);
                grid_num = 5;
                color = MyColorPalette.GoldenRod;
                break;
            case PolyominoPuzzleType.P5:
                shape = new int[3, 3] { { 1, 1, 0 }, { 1, 1, 0 }, { 1, 0, 0 } }; 
                size = 3;
                dim = new Vector2Int(3, 2);
                grid_num = 5;
                color = MyColorPalette.SpringGreen;
                break;
            case PolyominoPuzzleType.T5:
                shape = new int[3, 3] { { 1, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.YellowGreen;
                break;
            case PolyominoPuzzleType.U5:
                shape = new int[3, 3] { { 1, 0, 1 }, { 1, 1, 1 }, { 0, 0, 0 } };
                size = 3;
                dim = new Vector2Int(2, 3);
                grid_num = 5;
                color = MyColorPalette.ForestGreen;
                break;
            case PolyominoPuzzleType.V5:
                shape = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.Torquoise;
                break;
            case PolyominoPuzzleType.W5:
                shape = new int[3, 3] { { 0, 1, 1 }, { 1, 1, 0 }, { 1, 0, 0 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.Teal;
                break;
            case PolyominoPuzzleType.X5:
                shape = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.DeepSkyBlue;
                break;
            case PolyominoPuzzleType.Y5:
                shape = new int[4, 4] { { 1, 0, 0, 0 }, { 1, 1, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 } };
                size = 4;
                dim = new Vector2Int(4, 2);
                grid_num = 5;
                color = MyColorPalette.Navy;
                break;
            case PolyominoPuzzleType.Z5:
                shape = new int[3, 3] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 1 } };
                size = 3;
                dim = new Vector2Int(3, 3);
                grid_num = 5;
                color = MyColorPalette.Indigo;
                break;
            case PolyominoPuzzleType.O6:
                shape = new int[3, 3] { { 1, 1, 0 }, { 1, 1, 0 }, { 1, 1, 0 } };
                size = 3;
                dim = new Vector2Int(3, 2);
                grid_num = 6;
                color = MyColorPalette.DarkViolet;
                break;
            default:
                Debug.LogError("Wrong puzzle type!");
                break;
        }

        // update sprites
        transform.localScale = grid_util.GetGridScale();
        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = atlas_puzzles.GetSprite(obj_name);
        sprite_renderer.color = color;
        UpdateMesh();
    }

    public void OnObjectSpawn()
    {
        rotation = 0;
        flip = 0;
        position = new Vector2Int(-1, -1);
    }

    private void UpdateMesh()
    {
        // initialize collider
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.pathCount = 0;
        collider.pathCount = grid_num;

        float image_size = grid_util.image_size;
        List<Vector2> ref_vertices = new List<Vector2>()
        {
            Quaternion.Euler(0, 0, rotation * 90) * new Vector2(image_size, 0),
            Quaternion.Euler(0, 0, rotation * 90) * new Vector2(image_size, -image_size),
            Quaternion.Euler(0, 0, rotation * 90) * new Vector2(0, -image_size)
        };

        int path_idx = 0;
        Vector2 left_top_pos = GetTopLeftPosition();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (shape[i, j] == 1)
                {
                    Vector2[] path = new Vector2[4];
                    path[0] = new Vector2(j * image_size, -i * image_size);
                    path[0] = Quaternion.Euler(0, 0, rotation * 90) * path[0];
                    path[0] += left_top_pos;
                    int k = 1;
                    foreach (var ref_v in ref_vertices)
                    {
                        path[k] = path[0] + ref_v;
                        k++;
                    }
                    collider.SetPath(path_idx, path);
                    path_idx++;
                }
            }
        }
    }

    private Vector2 GetTopLeftPosition()
    {
        if (rotation == 0) // top left
            return new Vector2(-dim.y * grid_util.image_size * 0.5f, dim.x * grid_util.image_size * 0.5f);
        else if (rotation == 1) // bottom left
            return new Vector2(-dim.y * grid_util.image_size * 0.5f, -dim.x * grid_util.image_size * 0.5f);
        else if (rotation == 2) // bottom right
            return new Vector2(dim.y * grid_util.image_size * 0.5f, -dim.x * grid_util.image_size * 0.5f);
        else if (rotation == 3) // top right
            return new Vector2(dim.y * grid_util.image_size * 0.5f, dim.x * grid_util.image_size * 0.5f);
        return new Vector2(0f, 0f);
    }

    public void Rotate()
    {
        rotation = (rotation + 1) % 4;
        transform.rotation = Quaternion.Euler(0, 0, -rotation * 90);
        int[,] result = new int[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                result[j, size - 1 - i] = shape[i, j];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                shape[i, j] = result[i, j];

        ShiftTopLeft();
        UpdateMesh();

        if (dim.x != dim.y)
        {
            float grid_shift = 0.5f * ((rotation & 1) == 1 ? dim.x - dim.y : dim.y - dim.x);
            ShiftGrid(grid_shift);
        }
    }

    public void Flip()
    {
        if ((rotation & 1) == 0)
            sprite_renderer.flipX = !sprite_renderer.flipX;
        else
            sprite_renderer.flipY = !sprite_renderer.flipY;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size/2; j++)
            {
                (shape[i, size - 1 - j], shape[i, j]) = (shape[i, j], shape[i, size - 1 - j]);
            }
        }

        ShiftTopLeft();
        UpdateMesh();
    }

    public void PrintShape()
    {
        string s = "";
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                s += shape[i, j].ToString() + " \n"[(j == size - 1) ? 1 : 0];
        Debug.Log(s);
    }

    private void ShiftGrid(float grid_shift)
    {
        Vector3 curr_pos = Camera.main.WorldToScreenPoint(transform.position);
        curr_pos.x += grid_shift * grid_util.grid_width;
        curr_pos.y += grid_shift * grid_util.grid_width;
        transform.position = Camera.main.ScreenToWorldPoint(curr_pos);
    }

    private void ShiftTopLeft()
    {
        while (IsFirstRowEmpty())
        {
            ShiftUp();
        }
        while (IsFirstColumnEmpty())
        {
            ShiftLeft();
        }
    }

    private void ShiftLeft()
    {
        for (int i=0; i<size; i++)
        {
            for (int j=0; j<size-1; j++)
            {
                (shape[i, j + 1], shape[i, j]) = (shape[i, j], shape[i, j + 1]);
            }
        }
    }

    private void ShiftUp()
    {
        for (int i = 0; i < size-1; i++)
        {
            for (int j = 0; j < size; j++)
            {
                (shape[i + 1, j], shape[i, j]) = (shape[i, j], shape[i + 1, j]);
            }
        }
    }

    private bool IsFirstRowEmpty()
    {
        for (int i=0; i<size; i++)
        {
            if (shape[0, i] == 1) return false;
        }
        return true;
    }

    private bool IsFirstColumnEmpty()
    {
        for (int i = 0; i < size; i++)
        {
            if (shape[i, 0] == 1) return false;
        }
        return true;
    }

    public void Selected()
    {
        sprite_renderer.color = MyColorPalette.Red;
    }

    public void Unselected()
    {
        sprite_renderer.color = color;
    }

    public void UpdateDepth(float z)
    {
        Vector3 pos = gameObject.transform.position;
        pos.z = z;
        gameObject.transform.position = pos;
    }

    void OnMouseDown()
    {
        if (game_manager.edit_board_mode) return;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    }

    void OnMouseDrag()
    {
        if (game_manager.edit_board_mode) return;
        Vector3 pos = offset + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        pos.z = z_depth;
        transform.position = pos;
    }
    void OnMouseUp()
    {
        if (game_manager.edit_board_mode) return;
        MoveToGridPoint();
    }

    private void MoveToGridPoint()
    {
        Vector3 curr_pos = Camera.main.WorldToScreenPoint(transform.position);
        float x = ((rotation & 1) == 0) ? dim.y : dim.x;
        float y = ((rotation & 1) == 0) ? dim.x : dim.y;
        x = -x * grid_util.grid_width * 0.5f;
        y = y * grid_util.grid_width * 0.5f;
        transform.position = grid_util.ToGridPoint(curr_pos.x, curr_pos.y, out int row, out int col, transform.position, z_depth, x, y);
        position.x = row; position.y = col;
    }
}

