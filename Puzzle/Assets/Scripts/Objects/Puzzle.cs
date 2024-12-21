using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class Puzzle : MonoBehaviour, ISelectableObject, IPooledObject
{
    // instance
    public GameManager game_manager;
    public GridUtil grid_util;

    // parameters
    [HideInInspector] public int rotation;
    [HideInInspector] public int flip;
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public string obj_name;
    // properties
    [HideInInspector] public PuzzleType type;
    [HideInInspector] public int[,] shape;
    [HideInInspector] public int grid_num;
    [HideInInspector] public int size;
    [HideInInspector] public Vector2Int dim;
    [HideInInspector] public Color color;

    // hidden parameters
    [HideInInspector] public Vector3 offset = Vector3.zero;
    [HideInInspector] public float z_depth;
    [HideInInspector] public SpriteRenderer sprite_renderer;

    public virtual void Initialize(PuzzleType type, float z_depth, ref SpriteAtlas atlas_puzzles)
    {
        // initialize puzzle parameters
        rotation = 0;
        flip = 0;
        position = new Vector2Int(-1, -1);
        this.type = type;
        this.z_depth = z_depth;
        PuzzleProperty property = PuzzleProperties.GetProperty(type);
        shape = property.shape;
        grid_num = property.grid_num;
        size = property.size;
        dim = property.dim;
        color = property.color;

        // update sprites
        transform.localScale = grid_util.GetGridScale();
        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = atlas_puzzles.GetSprite(obj_name);
        sprite_renderer.color = color;
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
    }

    public void Flip()
    {
        if ((rotation & 1) == 0)
            sprite_renderer.flipX = !sprite_renderer.flipX;
        else
            sprite_renderer.flipY = !sprite_renderer.flipY;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size / 2; j++)
            {
                (shape[i, size - 1 - j], shape[i, j]) = (shape[i, j], shape[i, size - 1 - j]);
            }
        }

        ShiftTopLeft();
    }

    public void PrintShape()
    {
        string s = "";
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                s += shape[i, j].ToString() + " \n"[(j == size - 1) ? 1 : 0];
        Debug.Log(s);
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
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size - 1; j++)
            {
                (shape[i, j + 1], shape[i, j]) = (shape[i, j], shape[i, j + 1]);
            }
        }
    }

    private void ShiftUp()
    {
        for (int i = 0; i < size - 1; i++)
        {
            for (int j = 0; j < size; j++)
            {
                (shape[i + 1, j], shape[i, j]) = (shape[i, j], shape[i + 1, j]);
            }
        }
    }

    private bool IsFirstRowEmpty()
    {
        for (int i = 0; i < size; i++)
        {
            if (shape[0, i] != 0) return false;
        }
        return true;
    }

    private bool IsFirstColumnEmpty()
    {
        for (int i = 0; i < size; i++)
        {
            if (shape[i, 0] != 0) return false;
        }
        return true;
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

    public void UpdateDepth(float z)
    {
        Vector3 pos = gameObject.transform.position;
        pos.z = z;
        gameObject.transform.position = pos;
    }

    public void OnObjectSpawn()
    {
    }

    public void Selected()
    {
        sprite_renderer.color = MyColorPalette.Red;
    }

    public void Unselected()
    {
        sprite_renderer.color = color;
    }
}
