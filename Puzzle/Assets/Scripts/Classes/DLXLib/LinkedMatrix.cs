using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

// reference: https://github.com/taylorjg/DlxLib/tree/master
namespace DLXLib
{
    public class LinkedMatrix
    {
        /*
         *  Root -> col_0 -> col_1 -> col_2 -> col_3 -> col_4 -> Root
         *            |        |        |        |        |
         *          nodes    nodes    nodes    nodes    nodes
         */
        public Header Root { get; }

        public LinkedMatrix(bool[][] matrix)
        {
            Root = new Header();
            var column_headers = new List<Header>();

            // Header list
            for (int col = 0; col < matrix[0].Length; col++)
            {
                Header column_header = new Header();
                Root.AddColumnHeader(column_header);
                column_headers.Add(column_header);
            }

            // Matrix
            for (int row=0; row<matrix.Length; row++)
            {
                Node node_lead = null;
                for (int col=0; col<matrix[0].Length; col++)
                {
                    if (matrix[row][col])
                    {
                        Node node = new Node(column_headers[col], row);

                        if (node_lead != null)
                        {
                            node_lead.AppendToRow(node);
                        } else
                        {
                            node_lead = node;
                        }
                    }
                }
            }
        }

        public void Print()
        {
            string s = "Numbers of nodes in columns: ";
            for (Header header = Root.Next; header != Root; header = header.Next)
            {
                s += header.NumOfRows.ToString() + ",";
            }
            s += "\n";
            for (Header header = Root.Next; header != Root; header = header.Next)
            {
                for (Node node = header.Down; node != header; node = node.Down)
                {
                    s += node.RowIndex + ",";
                }
                s += "\n";
            }
        Debug.Log(s);
        }

        public bool Empty()
        {
            return Root.Next == Root;
        }

        public Header GetMinColumn()
        {
            Header min_column_header = null;
            for (Header column_header = Root.Next; column_header != Root; column_header = column_header.Next)
            {
                if (min_column_header == null || column_header.NumOfRows < min_column_header.NumOfRows)
                {
                    min_column_header = column_header;
                }
            }
            return min_column_header;
        }

        public void CoverColumn(Header column_header)
        {
            column_header.UnlinkColumnHeader();

            // Traverse all nodes in this column
            for (Node node_row = column_header.Down; node_row != column_header; node_row = node_row.Down)
            {
                // Traverse all nodes in this row
                for (Node node = node_row.Right; node != node_row; node = node.Right)
                {
                    node.ColumnHeader.UnlinkNode(node);
                }
            }
        }

        public void UncoverColumn(Header column_header)
        {
            column_header.RelinkColumnHeader();

            // Traverse all nodes in this column
            for (Node node_row = column_header.Down; node_row != column_header; node_row = node_row.Down)
            {
                // Traverse all nodes in this row
                for (Node node = node_row.Right; node != node_row; node = node.Right)
                {
                    node.ColumnHeader.RelinkNode(node);
                }
            }
        }
    }

    public class Header: Node
    {
        private Header Prev { get; set; }
        public Header Next { get; private set; }
        public int NumOfRows { get; private set; }

        public Header()
        {
            Prev = this;
            Next = this;
            NumOfRows = 0;
        }

        public void AddColumnHeader(Header header)
        {
            Prev.Next = header;
            header.Next = this;
            header.Prev = Prev;
            Prev = header;
        }

        public void AddNode(Node node)
        {
            AppendToColumn(node);
            NumOfRows++;
        }

        public void RelinkColumnHeader()
        {
            Prev.Next = this;
            Next.Prev = this;
        }

        public void UnlinkColumnHeader()
        {
            Prev.Next = Next;
            Next.Prev = Prev;
        }

        public void RelinkNode(Node node)
        {
            node.Relink();
            NumOfRows++;
        }

        public void UnlinkNode(Node node)
        {
            node.Unlink();
            NumOfRows--;
        }
    }

    public class Node
    {
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        public Node Up { get; private set; }
        public Node Down { get; private set; }

        public Header ColumnHeader { get; private set; }
        public int RowIndex { get; private set; }

        public Node (Header column_header, int idx)
        {
            Left = Right = Up = Down = this;
            ColumnHeader = column_header;
            RowIndex = idx;

            column_header?.AddNode(this);
        }

        protected Node (): this(null, -1)
        {
        }

        public void AppendToRow(Node node)
        {
            Left.Right = node;
            node.Right = this;
            node.Left = Left;
            Left = node;
        }

        public void AppendToColumn(Node node)
        {
            Up.Down = node;
            node.Down = this;
            node.Up = Up;
            Up = node;
        }

        public void Relink()
        {
            Up.Down = this;
            Down.Up = this;
        }

        public void Unlink()
        {
            Up.Down = Down;
            Down.Up = Up;
        }

    }
}
