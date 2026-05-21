using System.Collections;

public class MyLinkedList<T> : IMyCollection<T>
{
    private class Node
    {
        public T Data { get; set; }
        public Node? Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    private class LinkedListIterator : IMyIterator<T>
    {
        private Node? _current;
        private readonly Node? _head;

        public LinkedListIterator(Node? head)
        {
            _head = head;
            _current = head;
        }

        public bool HasNext()
        {
            return _current != null;
        }

        public T Next()
        {
            if (_current == null)
                throw new InvalidOperationException("No more elements");

            T data = _current.Data;
            _current = _current.Next;
            return data;
        }

        public void Reset()
        {
            _current = _head;
        }
    }

    private Node? _head;
    private int _count;
    private bool _dirty;

    public int Count => _count;
    public bool Dirty
    {
        get => _dirty;
        set => _dirty = value;
    }

    public MyLinkedList()
    {
        _head = null;
        _count = 0;
        _dirty = false;
    }

    public void Add(T item)
    {
        Node newNode = new Node(item);

        if (_head == null)
        {
            _head = newNode;
        }
        else
        {
            Node current = _head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }

        _count++;
        _dirty = true;
    }

    public void Remove(T item)
    {
        if (_head == null)
            return;

        if (_head.Data != null && _head.Data.Equals(item))
        {
            _head = _head.Next;
            _count--;
            _dirty = true;
            return;
        }

        Node current = _head;
        while (current.Next != null)
        {
            if (current.Next.Data != null && current.Next.Data.Equals(item))
            {
                current.Next = current.Next.Next;
                _count--;
                _dirty = true;
                return;
            }
            current = current.Next;
        }
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        Node? current = _head;

        while (current != null)
        {
            if (comparer(current.Data, key))
            {
                return current.Data;
            }
            current = current.Next;
        }

        return default;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyLinkedList<T> filtered = new MyLinkedList<T>();
        Node? current = _head;

        while (current != null)
        {
            if (predicate(current.Data))
            {
                filtered.Add(current.Data);
            }
            current = current.Next;
        }

        return filtered;
    }

    public void Sort(Comparison<T> comparison)
    {
        if (_count <= 1)
            return;

        T[] array = new T[_count];
        Node? current = _head;
        int index = 0;

        while (current != null)
        {
            array[index] = current.Data;
            index++;
            current = current.Next;
        }

        System.Array.Sort(array, comparison);

        _head = null;
        _count = 0;

        foreach (T item in array)
        {
            Add(item);
        }

        _dirty = true;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        return Reduce(default(R)!, accumulator);
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        Node? current = _head;

        while (current != null)
        {
            result = accumulator(result, current.Data);
            current = current.Next;
        }

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
        return new LinkedListIterator(_head);
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node? current = _head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }
    public T[] ToArray()
    {
        T[] result = new T[_count];
        int i = 0;
        Node? current = _head;
        while (current != null)
        {
            result[i++] = current.Data;
            current = current.Next;
        }
        return result;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}