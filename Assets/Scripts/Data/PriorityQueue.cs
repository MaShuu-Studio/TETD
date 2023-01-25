using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PriorityQueue<T>
{
    public int Count { get { return count; } }

    private int MAX_SIZE;
    private Node<T>[] array;
    private int count;

    public PriorityQueue()
    {
        MAX_SIZE = 100;
        array = new Node<T>[MAX_SIZE];
        count = 0;
    }

    public void Enqueue(T data, float priority)
    {
        Node<T> element = new Node<T>(data, priority);
        array[count++] = element;

        if (count > MAX_SIZE)
        {
            MAX_SIZE *= 2;
            Node<T>[] tmp = new Node<T>[MAX_SIZE];
            array.CopyTo(tmp, 0);
            array = null;
            array = tmp;
            GC.Collect();
        }

        int index = count - 1;

        while (index > 0)
        {
            int parentIndex = (int)((index + 1) / 2) - 1;
            Node<T> item = array[index];
            Node<T> parent = array[parentIndex];

            if (Compare(item, parent))
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
            else break;
        }
    }

    public T Dequeue()
    {
        if (count == 0) return default(T);

        T item = array[0].data;
        Heapify(0);

        return item;
    }

    private void Heapify(int index)
    {
        Swap(index, count - 1);
        count--;

        while (index < count - 1)
        {
            int left = (index + 1) * 2 - 1;
            int right = (index + 1) * 2;

            if (left > count - 1) break; // leaf가 없음
            else if (right > count - 1) index = left; //leaf가 left뿐임
            else if (Compare(array[left], array[right])) // left가 더 큼
            {
                Swap(index, left);
                index = left;
            }
            else
            {
                Swap(index, right);
                index = right;
            }
        }
    }

    public T Get()
    {
        if (count == 0) return default(T);

        return array[0].data;
    }

    public List<T> Get(int amount)
    {
        if (count == 0) return null;

        List<T> ts = new List<T>();
        for (int i = 0; i < amount && i < count; i++)
        {
            ts.Add(array[i].data);
        }
        return ts;
    }
    
    public void Remove(int amount)
    {
        if (amount > count) amount = count;
        for (int i = amount - 1; i >= 0; i--)
        {
            Heapify(i);
        }    
    }

    public bool Contains(T data)
    {
        for (int i = 0; i < count; i++)
        {
            if (array[i].data.Equals(data)) return true;
        }
        return false;
    }

    public void Remove(T data)
    {
        for (int i = 0; i < count; i++)
        {
            if (array[i].data.Equals(data))
            {
                Heapify(i);
                return;
            }
        }
    }

    private bool Compare(Node<T> first, Node<T> second)
    {
        return first.priority > second.priority;
    }

    private void Swap(int first, int second)
    {
        Node<T> tmp = array[first];
        array[first] = array[second];
        array[second] = tmp;
    }
}

public class Node<T>
{
    public T data;
    public float priority;

    public Node(T node, float priority)
    {
        this.data = node;
        this.priority = priority;
    }
}
