public interface IMyHashMap<TKey, TValue>
{
    void Add(TKey key, TValue value);
    bool Remove(TKey key);
    TValue? FindBy(TKey key);
    bool ContainsKey(TKey key);
    int Count { get; }
    bool Dirty { get; set; }
    IMyIterator<KeyValuePair<TKey, TValue>> GetIterator();
}