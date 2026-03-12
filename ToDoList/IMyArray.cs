public interface IMyArray<T>
{
    int Count {get;}
    bool Dirty {get; set;}
    void Add(T item); //return index
    void Remove(T item);
    T FindBy<K>(K key, Func<T, K, int> comparer);
    IMyArray<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
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