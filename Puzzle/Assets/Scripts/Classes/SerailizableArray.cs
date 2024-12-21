using System;

[Serializable]
public class SerializableArray
{
    public int size;
    public ArrayRow[] array;

    public SerializableArray(int[,] arr)
    {
        size = arr.GetLength(0);
        array = new ArrayRow[size];
        for (int i=0; i<size; i++)
        {
            array[i] = new ArrayRow(arr, i, size);
        }
    }

    public int[,] GetArray()
    {
        int[,] result = new int[size, size];
        for (int i=0; i<size; i++)
        {
            for (int j=0; j<size; j++)
            {
                result[i, j] = array[i].row[j];
            }
        }
        return result;
    }
}


[Serializable]
public class ArrayRow
{
    public int[] row;

    public ArrayRow (int[,] arr, int i, int size)
    {
        row = new int[size];
        for (int j=0; j<size; j++)
        {
            row[j] = arr[i, j];
        }
    }
}