public class MyArray<T> : IMyArray<T>
{
    private T[] _items = new T[0];
    private int _count = 0;

    public int Count
    {
        get { return _count; }
    }

    public bool Dirty { get; set; }

    private class MyIterator : IMyIterator<T>
    {
        private readonly T[] _data;
        private readonly int _count;
        private int _index = 0;

        public MyIterator(T[] data, int count)
        {
            _data = data;
            _count = count;
        }

        public bool HasNext()
        {
            return _index < _count;
        }

        public T Next()
        {
            T item = _data[_index];
            _index++;
            return item;
        }

        public void Reset()
        {
            _index = 0;
        }
    }

    public void Add(T item)
    {
        if (_count == _items.Length)
        {
            int newSize = _items.Length == 0 ? 1 : _items.Length * 2;
            T[] bigger = new T[newSize];
            for (int i = 0; i < _count; i++)
            {
                bigger[i] = _items[i];
            }
            _items = bigger;
        }

        _items[_count] = item;
        _count++;
        Dirty = true;
    }

    public void Remove(T item)
    {
        int index = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_items[i].Equals(item))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            return;
        }

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _count--;
        _items[_count] = default;
        Dirty = true;
    }

    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        for (int i = 0; i < _count; i++)
        {
            if (comparer(_items[i], key) == 0)
            {
                return _items[i];
            }
        }
        return default;
    }

    public IMyArray<T> Filter(Func<T, bool> predicate)
    {
        MyArray<T> result = new MyArray<T>();
        IMyIterator<T> iterator = GetIterator();

        while (iterator.HasNext())
        {
            T item = iterator.Next();
            if (predicate(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        for (int i = 1; i < _count; i++)
        {
            T key = _items[i];
            int j = i - 1;

            while (j >= 0 && comparison(_items[j], key) > 0)
            {
                _items[j + 1] = _items[j];
                j--;
            }

            _items[j + 1] = key;
        }
        Dirty = true;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        R result = default;
        for (int i = 0; i < _count; i++)
        {
            result = accumulator(result, _items[i]);
        }
        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        for (int i = 0; i < _count; i++)
        {
            result = accumulator(result, _items[i]);
        }
        return result;
    }

    public IMyIterator<T> GetIterator()
    {
        return new MyIterator(_items, _count);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }
}