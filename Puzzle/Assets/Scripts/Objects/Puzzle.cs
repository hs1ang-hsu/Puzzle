using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Puzzle : MonoBehaviour, ISelectableObject
{
    public int[,] shape;
    public int size;
    public string obj_name;
    public Color color;
    public PuzzleType type;
    public List<List<GameObject>> children;
    public Vector2Int position;

    private Vector3 offset = Vector3.zero;
    private float z_depth;
    private GameManager GM;
    private GridUtil grid_util;

    private void Start()
    {
        GM = GameManager.instance;
        grid_util = GridUtil.instance;
    }

    public void Initialize(PuzzleType type, float z_depth)
    {
        this.obj_name = "Puzzle_" + type.ToString();
        this.type = type;
        this.z_depth = z_depth;
        position = new Vector2Int(-1, -1);
        size = 3;
        switch (type)
        {
            case PuzzleType.C:
                shape = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 1 }, { 0, 0, 0 } };
                color = new Color(1f, 0.6f, 0.2f);
                break;
            case PuzzleType.O:
                shape = new int[3, 3] { { 1, 1, 0 }, { 1, 1, 0 }, { 1, 1, 0 } };
                color = new Color(1f, 0.6f, 0.8f);
                break;
            case PuzzleType.T:
                shape = new int[4, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
                size = 4;
                color = new Color(0.945f, 0.576f, 0.396f);
                break;
            case PuzzleType.Z:
                shape = new int[3, 3] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 1 } };
                color = new Color(0.949f, 0.933f, 0.443f);
                break;
            case PuzzleType.V:
                shape = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 } };
                color = new Color(0.451f, 0.753f, 0.522f);
                break;
            case PuzzleType.L:
                shape = new int[4, 4] { { 1, 1, 1, 1 }, { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
                size = 4;
                color = new Color(0.408f, 0.557f, 0.788f);
                break;
            case PuzzleType.S:
                shape = new int[4, 4] { { 1, 1, 1, 0 }, { 0, 0, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
                size = 4;
                color = new Color(0.533f, 0.808f, 0.851f);
                break;
            case PuzzleType.B:
                shape = new int[3, 3] { { 1, 1, 1 }, { 1, 1, 0 }, { 0, 0, 0 } };
                color = new Color(0.459f, 0.392f, 0.667f);
                break;
            default:
                Debug.LogError("Wrong puzzle type!");
                break;
        }
    }
    
    private void UpdateShape()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                children[i][j].SetActive(shape[i, j] == 1);
            }
        }
    }

    public void Rotate()
    {
        int[,] result = new int[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                result[j, size - 1 - i] = shape[i, j];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                shape[i, j] = result[i, j];


        ShiftTopLeft();
        UpdateShape();
    }

    public void Flip()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size/2; j++)
            {
                (shape[i, size - 1 - j], shape[i, j]) = (shape[i, j], shape[i, size - 1 - j]);
            }
        }

        ShiftTopLeft();
        UpdateShape();
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
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                children[i][j].transform.Find("Highlight").gameObject.SetActive(true);
            }
        }
    }

    public void Unselected()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                children[i][j].transform.Find("Highlight").gameObject.SetActive(false);
            }
        }
    }

    public void UpdateDepth(float z)
    {
        Vector3 pos = gameObject.transform.position;
        pos.z = z;
        gameObject.transform.position = pos;
    }

    void OnMouseDown()
    {
        if (GM.edit_board_mode) return;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    }

    void OnMouseDrag()
    {
        if (GM.edit_board_mode) return;
        Vector3 pos = offset + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        pos.z = z_depth;
        transform.position = pos;
    }
    void OnMouseUp()
    {
        if (GM.edit_board_mode) return;

        Vector3 curr_pos = Camera.main.WorldToScreenPoint(transform.position);
        transform.position = grid_util.ToGridPoint(curr_pos.x, curr_pos.y, out int row, out int col, transform.position, z_depth);
        position.x = row; position.y = col;
    }
}

