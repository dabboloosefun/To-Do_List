public class MyBinarySearchTree<T> : IMyCollection<T>
{
    private class Node
    {
        public T data;
        public Node? root;
        public Node? left;
        public Node? right;

        public Node(T Data, Node? Root = null)
        {
            data = Data;
            root = Root;
            left = null;
            right = null;
        }
    }
    
    private Node? _head;
    private int _count = 0;
    private bool _dirty = false;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
        }
    }
    public bool Dirty
    {
        get => _dirty;
        set
        {
            _dirty = value;
        }
    }

    public MyBinarySearchTree(T? head)
    {
        _head = head == null ? null : new Node(head);
    }

    void Add(T item)
    {
        Node node = new Node(item);
        
    }
    void Remove(T item);
    T FindBy<K>(K key, Func<T, K, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    int Count { get; }
    bool Dirty { get; set; }
    R Reduce<R>(Func<R, T, R> accumulator);
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator();
    IEnumerator<T> GetEnumerator();
}