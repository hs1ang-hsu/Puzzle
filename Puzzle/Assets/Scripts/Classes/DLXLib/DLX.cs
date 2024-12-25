using DLXLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// reference: https://github.com/taylorjg/DlxLib/tree/master
namespace DLXLib
{
    public class DLX
    {
        public LinkedMatrix Matrix { get; private set; }

        public List<int> Solution { get; private set; }

        public Stack<int> CurrentSolution { get; private set; }

        public bool Solved { get; private set; }

        public int iteration = 0;
        private int speed = 1000;

        public DLX(bool[][] matrix)
        {
            Matrix = new LinkedMatrix(matrix);
            Solution = new List<int>();
            CurrentSolution = new Stack<int>();
            Solved = false;
        }

        public IEnumerator Search(int k)
        {
            if (Matrix.Empty())
            {
                Solved = true;
                yield break;
            }

            iteration++;
            if (iteration % speed == 0)
            {
                yield return null;
            }

            Header min_column_header = Matrix.GetMinColumn();
            Matrix.CoverColumn(min_column_header);

            for (Node node_row = min_column_header.Down; node_row != min_column_header; node_row = node_row.Down)
            {
                CurrentSolution.Push(node_row.RowIndex);
                for (Node node = node_row.Right; node != node_row; node = node.Right)
                    Matrix.CoverColumn(node.ColumnHeader);

                IEnumerator search = Search(k + 1);
                while (search.MoveNext())
                {
                    if (iteration % speed == 0)
                    {
                        yield return null;
                    }
                }
                
                if (Solved)
                {
                    yield break;
                }
                
                for (Node node = node_row.Right; node != node_row; node = node.Right)
                    Matrix.UncoverColumn(node.ColumnHeader);
                CurrentSolution.Pop();
            }

            Matrix.UncoverColumn(min_column_header);
        }
    }
}
