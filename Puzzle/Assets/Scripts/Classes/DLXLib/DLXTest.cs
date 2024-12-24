using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DLXLib
{
    public class DLXTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            List<bool[]> matrix = new List<bool[]>()
            {
                new bool[5] { true, false, false, true, false},
                new bool[5] { false, true, false, false, true},
                new bool[5] { false, false, true, false, true},
                new bool[5] { false, true, false, false, false}
            };
            DLX solver = new DLX(matrix.ToArray());
            //solver.Matrix.Print();

            //StartCoroutine(solver.Search(0));
            //Debug.Log(solver.Solved);
            //List<int> solution = new List<int>();
            //solution = solver.CurrentSolution.ToList();
            //string s = "";
            //foreach (int sol in solution)
            //{
            //    s += sol.ToString() + ",";
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
