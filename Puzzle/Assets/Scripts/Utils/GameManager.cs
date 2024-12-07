using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool editor_mode;
    public static GameManager instance;

    // instance
    private PuzzleUtil puzzle_util;

    // mouse events
    [HideInInspector]
    public bool edit_board_mode = false;
    public Image img_edit_board;
    public Button btn_rotate;
    public Button btn_flip;

    [HideInInspector]
    public Transform selected = null;

    // GameObject
    public GameObject obj_board;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        puzzle_util = PuzzleUtil.instance;
    }

    // Update is called once per frame
    void Update()
    {
        btn_rotate.gameObject.SetActive(selected != null && selected.CompareTag("puzzle"));
        btn_flip.gameObject.SetActive(selected != null && selected.CompareTag("puzzle"));

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouse_pos, Vector2.zero);
            if (hits.Count() == 0)
            {
                UnSelectedObject(null);
            }
            else
            {
                OnRaycastHit(hits[0]);
            }
        }
    }

    private void UnSelectedObject(Transform new_selected)
    {
        if (selected == null)
        {
            selected = new_selected;
            return;
        }

        ISelectableObject selectable_obj = selected.GetComponent<ISelectableObject>();
        if (selectable_obj != null)
            selectable_obj.Unselected();
        selected = new_selected;
    }

    private void OnRaycastHit(RaycastHit2D hit)
    {
        Transform obj_hit = hit.transform;
        if (obj_hit.CompareTag("UI button"))
        {
            return;
        } else if (obj_hit.CompareTag("grid"))
        {
            Transform obj_hit_parent = obj_hit.parent;
            if (obj_hit_parent == null)
            {
                UnSelectedObject(null);
                return;
            }

            if (edit_board_mode && obj_hit_parent.CompareTag("board"))
            {
                obj_board.GetComponent<Board>().EditBoard(obj_hit);
            }
            UnSelectedObject(null);
            return;
        } else if (obj_hit.CompareTag("puzzle"))
        {
            if (edit_board_mode || selected == obj_hit) return;

            puzzle_util.SortPuzzle(obj_hit.gameObject);
        }

        UnSelectedObject(obj_hit);
        ISelectableObject selectable_obj = selected.GetComponent<ISelectableObject>();
        selectable_obj?.Selected();
    }

    public void OnButtonEditBoardClicked()
    {
        if (!edit_board_mode)
        {
            UnSelectedObject(null);
            puzzle_util.InitializePuzzlePos();
            obj_board.GetComponent<Board>().ResetBoard();
            obj_board.GetComponent<Board>().Selected();
            edit_board_mode = true;
            img_edit_board.color = new Color(0.8f, 0.8f, 0.8f);
            return;
        }

        obj_board.GetComponent<Board>().Unselected();
        img_edit_board.color = Color.white;
        edit_board_mode = false;
    }

    public void OnButtonFlipClicked()
    {
        if (!selected.CompareTag("puzzle"))
        {
            Debug.LogError("Flip the wrong GameObject!");
            return;
        }
        selected.GetComponent<Puzzle>().Flip();
    }

    public void OnButtonRotateClicked()
    {
        if (!selected.CompareTag("puzzle"))
        {
            Debug.LogError("Rotate the wrong GameObject!");
            return;
        }
        selected.GetComponent<Puzzle>().Rotate();
    }
}
