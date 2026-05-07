using System;
using System.Collections;
using System.Collections.Generic;

public class MyHashMap<T> : IMyCollection<T> where T : IHasId
{
    private class Entry
    {
        public int Key { get; set; }
        public T Value { get; set; }
        public Entry? Next { get; set; }

        public Entry(int key, T value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    private class HashMapIterator : IMyIterator<T>
    {
        private readonly Entry?[] _buckets;
        private int _bucketIndex;
        private Entry? _current;
        private readonly int _capacity;

        public HashMapIterator(Entry?[] buckets, int capacity)
        {
            _buckets = buckets;
            _capacity = capacity;
            _bucketIndex = -1;
            _current = null;
            MoveToNextAvailable();
        }

        private void MoveToNextAvailable()
        {
            while (_current == null && ++_bucketIndex < _capacity)
            {
                _current = _buckets[_bucketIndex];
            }
        }

        public bool HasNext()
        {
            return _current != null;
        }

        public T Next()
        {
            if (_current == null)
                throw new InvalidOperationException("No more elements");

            T value = _current.Value;
            _current = _current.Next;

            if (_current == null)
            {
                MoveToNextAvailable();
            }

            return value;
        }

        public void Reset()
        {
            _bucketIndex = -1;
            _current = null;
            MoveToNextAvailable();
        }
    }

    private readonly Entry?[] _buckets;
    private int _count;
    private bool _dirty;

    public int Count => _count;
    public bool Dirty
    {
        get => _dirty;
        set => _dirty = value;
    }

    public MyHashMap(int capacity = 101)
    {
        _buckets = new Entry?[capacity];
        _count = 0;
        _dirty = false;
    }

    private int GetBucketIndex(int key)
    {
        int hash = key.GetHashCode();
        if (hash < 0) hash = -hash;
        return hash % _buckets.Length;
    }

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        Add(item.Id, item);
    }

    public void Add(int key, T item)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];

        while (current != null)
        {
            if (current.Key == key)
            {
                current.Value = item;
                _dirty = true;
                return;
            }
            current = current.Next;
        }

        Entry newEntry = new Entry(key, item)
        {
            Next = _buckets[index]
        };

        _buckets[index] = newEntry;
        _count++;
        _dirty = true;
    }

    public void Remove(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        Remove(item.Id);
    }

    public bool Remove(int key)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];
        Entry? previous = null;

        while (current != null)
        {
            if (current.Key == key)
            {
                if (previous == null)
                {
                    _buckets[index] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                _count--;
                _dirty = true;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));

        for (int i = 0; i < _buckets.Length; i++)
        {
            Entry? current = _buckets[i];
            while (current != null)
            {
                if (comparer(current.Value, key))
                    return current.Value;
                current = current.Next;
            }
        }

        return default;
    }

    public T? FindBy(int key)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];
        while (current != null)
        {
            if (current.Key == key) return current.Value;
            current = current.Next;
        }
        return default;
    }

    public IMyIterator<T> GetIterator()
    {
        return new HashMapIterator(_buckets, _buckets.Length);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _buckets.Length; i++)
        {
            Entry? current = _buckets[i];
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var result = new MyHashMap<T>(_buckets.Length);
        foreach (var item in this)
        {
            if (predicate(item)) result.Add(item);
        }
        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        if (comparison == null) throw new ArgumentNullException(nameof(comparison));
        var list = new List<T>();
        foreach (var item in this) list.Add(item);
        list.Sort(comparison);

        for (int i = 0; i < _buckets.Length; i++) _buckets[i] = null;
        _count = 0;
        foreach (var item in list) Add(item);
        _dirty = true;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
        R result = default!;
        foreach (var item in this)
        {
            result = accumulator(result, item);
        }
        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
        R result = initial;
        foreach (var item in this)
        {
            result = accumulator(result, item);
        }
        return result;
    }

    public T[] ToArray()
    {
        T[] result = new T[_count];
        int i = 0;
        for (int b = 0; b < _buckets.Length; b++)
        {
            Entry? current = _buckets[b];
            while (current != null)
            {
                result[i++] = current.Value;
                current = current.Next;
            }
        }
        return result;
    }

}