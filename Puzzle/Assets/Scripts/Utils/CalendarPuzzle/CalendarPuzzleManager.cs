using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CalendarPuzzleManager : MonoBehaviour
{
    // instance
    public PuzzleUtil puzzle_util;
    public GridUtil grid_util;
    public GameManager game_manager;
    public Board board;

    // board settings
    public SerializableArray _calendar_puzzle_board;
    private int[,] calendar_puzzle_board;
    private Vector2Int board_dim;
    public int removed_limit = 2;
    public float z_depth = 20f;

    // puzzle settings
    private bool initialized = false;
    public List<PuzzleType> required_puzzle_types;
    public List<Vector2> initial_pos;

    // puzzle solver
    private CalendarPuzzleSolver calendar_puzzle_solver;

    // screen size
    public GameObject obj_camera;
    private Vector2 resolution;
    private bool alive = true;
    private float check_delay = 0.5f;

    // UI
    public Image img_edit_board;
    public Button btn_edit_board;
    public Button btn_rotate;
    public Button btn_flip;
    public Button btn_solve;
    public GameObject LoadingPanel;

    [HideInInspector]
    public Transform selected = null;

    // GameObject
    public GameObject obj_board;

    // Start is called before the first frame update
    void Start()
    {
        calendar_puzzle_solver = new CalendarPuzzleSolver();
        calendar_puzzle_board = _calendar_puzzle_board.GetArray();
        board_dim = _calendar_puzzle_board.GetSize();
        StartCoroutine(DetectScreenSizeChanged());
    }

    // Update is called once per frame
    void Update()
    {
        // Initialize board first (grids are also initialized here)
        if (!board.initialized)
        {
            StartCoroutine(board.InitializeBoard(calendar_puzzle_board, board_dim, removed_limit, z_depth));
        }

        // After grids are initialized, we generate puzzles
        if (!initialized && grid_util.initialized)
        {
            initialized = true;
            puzzle_util.GeneratePuzzles(required_puzzle_types, initial_pos);
        }

        // Update UI
        UpdateUI();

        // Mouse events
        if (!game_manager.freeze_all && Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouse_pos, Vector2.zero);

            RaycastHit test = new RaycastHit();
            if (Physics.Raycast(mouse_pos, Vector3.forward, out test, Mathf.Infinity))
            {
                Debug.Log(test.transform.name);
            }
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

    private IEnumerator DetectScreenSizeChanged()
    {
        resolution = new Vector2(Screen.width, Screen.height);
        while (alive)
        {
            // Check for resolution change
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                obj_camera.GetComponent<CameraManager>().InitializeResolution();
                resolution = new Vector2(Screen.width, Screen.height);
                grid_util.OnScreenSizeChanged();
                board.ResetBoard(true);
                puzzle_util.OnScreenSizeChanged();
            }
            yield return new WaitForSeconds(check_delay);
        }
    }

    private void UpdateUI()
    {
        LoadingPanel.SetActive(game_manager.freeze_all);
        btn_edit_board.interactable = !game_manager.freeze_all;
        btn_rotate.interactable = !game_manager.freeze_all;
        btn_flip.interactable = !game_manager.freeze_all;
        btn_solve.interactable = !game_manager.freeze_all;

        btn_rotate.gameObject.SetActive(selected != null && selected.CompareTag("puzzle"));
        btn_flip.gameObject.SetActive(selected != null && selected.CompareTag("puzzle"));
        btn_solve.gameObject.SetActive(!game_manager.edit_board_mode);
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

            if (game_manager.edit_board_mode && obj_hit_parent.CompareTag("board"))
            {
                obj_board.GetComponent<Board>().EditBoard(obj_hit);
            }
            UnSelectedObject(null);
            return;
        } else if (obj_hit.CompareTag("puzzle"))
        {
            if (game_manager.edit_board_mode || selected == obj_hit) return;

            puzzle_util.SortPuzzle(obj_hit.gameObject);
        }

        UnSelectedObject(obj_hit);
        ISelectableObject selectable_obj = selected.GetComponent<ISelectableObject>();
        selectable_obj?.Selected();
    }

    public void OnButtonEditBoardClicked()
    {
        if (!game_manager.edit_board_mode)
        {
            UnSelectedObject(null);
            puzzle_util.InitializePuzzlePos();
            obj_board.GetComponent<Board>().ResetBoard();
            obj_board.GetComponent<Board>().Selected();
            game_manager.edit_board_mode = true;
            img_edit_board.color = new Color(0.8f, 0.8f, 0.8f);
            return;
        }

        obj_board.GetComponent<Board>().Unselected();
        img_edit_board.color = Color.white;
        game_manager.edit_board_mode = false;
    }

    public void OnButtonFlipClicked()
    {
        if (!selected.CompareTag("puzzle"))
        {
            Debug.LogError("Flip the wrong GameObject!");
            return;
        }
        selected.GetComponent<PolyominoPuzzle>().Flip();
    }

    public void OnButtonRotateClicked()
    {
        if (!selected.CompareTag("puzzle"))
        {
            Debug.LogError("Rotate the wrong GameObject!");
            return;
        }
        selected.GetComponent<PolyominoPuzzle>().Rotate();
    }

    public void OnButtonSolveClicked()
    {
        int[,] state = board.GetPolyominoPuzzleBoardState(out bool is_board_valid, out List<Puzzle> unused_puzzles);
        if (!is_board_valid)
        {
            // show dialog
            throw new NotImplementedException();
        }

        List<PolyominoPuzzle> puzzles = new List<PolyominoPuzzle>();
        foreach (var puzzle in unused_puzzles)
        {
            puzzles.Add((PolyominoPuzzle) puzzle);
        }

        Debug.Log("Start solving");
        game_manager.freeze_all = true;
        StartCoroutine(calendar_puzzle_solver.Solve(state, puzzles, () =>
        {
            foreach (PolyominoPuzzle puzzle in calendar_puzzle_solver.puzzles)
            {
                puzzle.UpdateState();
            }
            game_manager.freeze_all = false;
        }));
    }

    private void OnDestroy()
    {
        alive = false;
    }
}
