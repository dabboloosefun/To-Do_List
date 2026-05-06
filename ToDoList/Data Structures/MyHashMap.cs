using System;
using System.Collections;
using System.Collections.Generic;

public class MyHashMap<TKey, TValue> : IMyHashMap<TKey, TValue>
{
    private class Entry
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public Entry? Next { get; set; }

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    private class HashMapIterator : IMyIterator<KeyValuePair<TKey, TValue>>
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

        public KeyValuePair<TKey, TValue> Next()
        {
            if (_current == null)
                throw new InvalidOperationException("No more elements");

            KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
            _current = _current.Next;

            if (_current == null)
            {
                MoveToNextAvailable();
            }

            return pair;
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

    private int GetBucketIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        if (hash < 0) hash = -hash;
        return hash % _buckets.Length;
    }

    public void Add(TKey key, TValue value)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                current.Value = value;
                _dirty = true;
                return;
            }
            current = current.Next;
        }

        Entry newEntry = new Entry(key, value)
        {
            Next = _buckets[index]
        };

        _buckets[index] = newEntry;
        _count++;
        _dirty = true;
    }

    public bool Remove(TKey key)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];
        Entry? previous = null;

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
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

    public TValue? FindBy(TKey key)
    {
        int index = GetBucketIndex(key);
        Entry? current = _buckets[index];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                return current.Value;
            }
            current = current.Next;
        }

        return default;
    }

    public bool ContainsKey(TKey key)
    {
        return FindBy(key) != null;
    }

    public IMyIterator<KeyValuePair<TKey, TValue>> GetIterator()
    {
        return new HashMapIterator(_buckets, _buckets.Length);
    }
}