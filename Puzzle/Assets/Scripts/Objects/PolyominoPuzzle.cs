using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;


public class PolyominoPuzzle : Puzzle
{
    public override void Initialize(PuzzleType type, float z_depth, ref SpriteAtlas atlas_puzzles)
    {
        // initialize puzzle parameters
        this.obj_name = "polyomino_" + type.ToString();
        if ((type & PuzzleType.Polyomino) == 0)
        {
            Debug.LogError("Wrong puzzle type!");
        }

        base.Initialize(type, z_depth, ref atlas_puzzles);
        UpdateMesh();
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

    public new void Rotate()
    {
        base.Rotate();

        UpdateMesh();
        if (dim.x != dim.y)
        {
            float grid_shift = 0.5f * ((rotation & 1) == 1 ? dim.x - dim.y : dim.y - dim.x);
            ShiftGrid(grid_shift);
        }
    }

    public new void Flip()
    {
        base.Flip();

        UpdateMesh();
    }

    private void ShiftGrid(float grid_shift)
    {
        Vector3 curr_pos = Camera.main.WorldToScreenPoint(transform.position);
        curr_pos.x += grid_shift * grid_util.grid_width;
        curr_pos.y += grid_shift * grid_util.grid_width;
        transform.position = Camera.main.ScreenToWorldPoint(curr_pos);
    }


    //void OnMouseDown()
    //{
    //    if (game_manager.edit_board_mode) return;
    //    offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    //}

    //void OnMouseDrag()
    //{
    //    if (game_manager.edit_board_mode) return;
    //    Vector3 pos = offset + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    //    pos.z = z_depth;
    //    transform.position = pos;
    //}

    //void OnMouseUp()
    //{
    //    if (game_manager.edit_board_mode) return;
    //    MoveToGridPoint();
    //}

    //private void MoveToGridPoint()
    //{
    //    Vector3 curr_pos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = ((rotation & 1) == 0) ? dim.y : dim.x;
    //    float y = ((rotation & 1) == 0) ? dim.x : dim.y;
    //    x = -x * grid_util.grid_width * 0.5f;
    //    y = y * grid_util.grid_width * 0.5f;
    //    transform.position = grid_util.ToGridPoint(curr_pos.x, curr_pos.y, out int row, out int col, transform.position, z_depth, x, y);
    //    position.x = row; position.y = col;
    //}
}

