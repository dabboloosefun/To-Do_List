public class MyBinarySearchTree<T> : IMyCollection<T>
{
    private class Comparer
    {
        private readonly Func<T, T, int> _comparison;

        public Comparer(Func<T, T, int> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T a, T b) => _comparison(a, b);
    }

    private class Node
    {
        public T data;
        public Node? parent = null;
        public Node? left = null;
        public Node? right = null;

        public Node(T Data)
        {
            data = Data;
        }
    }
    
    private Node? _root;
    private readonly Comparer _comparer;
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

    public MyBinarySearchTree(T? head, Func<T, T, int> comparison)
    {
        _root = head == null ? null : new Node(head);
        _comparer = new Comparer(comparison);
    }

    public void Add(T data)
    {
        _root = AddRecursive(data, _root);
    }
    private Node? AddRecursive(T data, Node? current)
    {
        if (current is null)
        {
            _count += 1;
            _dirty = true;
            return new Node(data);
        }

        int comparison = _comparer.Compare(data, current.data);

        if (comparison == 0) return current;

        if (comparison < 0)
        {
            current.left = AddRecursive(data, current.left);
        }

        if (comparison > 0)
        {
            current.right = AddRecursive(data, current.right);
        }

        return current;
    }

    public void Remove(T data)
    {
        if (_root == null) return;
        _root = RemoveRecursive(data, _root);
    }

    private Node? RemoveRecursive(T data, Node? current)
    {
        if (current == null) return null;
        int comparison = _comparer.Compare(data, current.data);

        if (comparison < 0) current.left = RemoveRecursive(data, current.left);

        if (comparison > 0) current.right = RemoveRecursive(data, current.right);

        if (comparison == 0)
        {
            if (current.left == null && current.right == null) current = null;

            else if (current.right == null) current = current.left;

            else if (current.left == null) current = current.right;

            else
            {
                Node currentChild = current.right; 
                while (currentChild.left != null)
                {
                    currentChild = currentChild.left;
                }
                current.data = currentChild.data;
                current.right = RemoveRecursive(currentChild.data, current.right);
            }
        }
        _dirty = true;
        _count -= 1;
        return current;
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        if (_root == null) return default;
        return FindByRecursive<K>(key, comparer, _root);

    private T? FindByRecursive<K>(K key, Func<T, K, bool> comparer, Node? current)
    {
        if (current == null) return default;

        T? result = default;
        if (comparer(current.data, key))
        {
            result = current.data;
        }

        if (result == null) result = FindByRecursive<K>(key, comparer, current.left);

        if (result == null) result = FindByRecursive<K>(key, comparer, current.right);

        return result;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate);
    public void Sort(Comparison<T> comparison);
    public int Count { get; }
    public bool Dirty { get; set; }
    public R Reduce<R>(Func<R, T, R> accumulator);
    public R Reduce<R>(R initial, Func<R, T, R> accumulator);
    public IMyIterator<T> GetIterator();
    public IEnumerator<T> GetEnumerator();
}