using System;
using UnityEngine;

[Serializable]
public class SerializableArray
{
    public Vector2Int size;
    public ArrayRow[] array;

    public SerializableArray(int[,] arr)
    {
        size = new Vector2Int(arr.GetLength(0), arr.GetLength(1));
        array = new ArrayRow[size.x];
        for (int i=0; i<size.x; i++)
        {
            array[i] = new ArrayRow(arr, i, size.y);
        }
    }

    public int[,] GetArray()
    {
        int[,] result = new int[size.x, size.y];
        for (int i=0; i<size.x; i++)
        {
            for (int j=0; j<size.y; j++)
            {
                result[i, j] = array[i].row[j];
            }
        }
        return result;
    }

    public Vector2Int GetSize()
    {
        return size;
    }
}


[Serializable]
public class ArrayRow
{
    public int[] row;

    public ArrayRow (int[,] arr, int i, int row_size)
    {
        row = new int[row_size];
        for (int j=0; j< row_size; j++)
        {
            row[j] = arr[i, j];
        }
    }
}