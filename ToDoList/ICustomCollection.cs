public interface IMyCollection<T>
{
    void Add(T item); //return index
    void Remove(T item);
    T FindBy<K>(K key, Func<T, K, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    int Count {get;}
    bool Diry {get; set;}
    R Reduce<R>(Func<R ,T ,R> accumulator);
    //OR
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator();
    IEnumerator<T> GetEnumerator();
}

public interface IMyIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}