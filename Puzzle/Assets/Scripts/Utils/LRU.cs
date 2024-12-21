using System.Collections.Generic;
using UnityEngine;

public class LRU<T>
{
    private LinkedList<T> list;
    private Dictionary<T, LinkedListNode<T>> map;
    private int capacity;
    private T default_value;

    public LRU (int capacity, T default_value = default)
    {
        this.capacity = capacity;
        this.default_value = default_value;
        list = new LinkedList<T>();
        map = new Dictionary<T, LinkedListNode<T>>();
    }

    public T Put(T item)
    {
        if (map.TryGetValue(item, out var curr))
        {
            list.Remove(curr);
            list.AddFirst(curr);
            return item;
        }

        T removed = default_value;
        if (list.Count == capacity)
        {
            removed = list.Last.Value;
            map.Remove(list.Last.Value);
            list.RemoveLast();
        }
        list.AddFirst(item);
        map.Add(item, list.First);
        return removed;
    }

    public void Update(T item)
    {
        if (map.TryGetValue(item, out var curr))
        {
            list.Remove(curr);
            list.AddFirst(curr);
        }
    }

    public void Remove(T item)
    {
        if (map.TryGetValue(item, out var curr))
        {
            map.Remove(item);
            list.Remove(curr);
        }
    }

    public void Clear()
    {
        map.Clear();
        list.Clear();
    }

    public T[] GetList()
    {
        T[] result = new T[list.Count];
        list.CopyTo(result, 0);
        return result;
    }
}
