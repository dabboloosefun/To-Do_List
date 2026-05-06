using System.Text.Json;

class JsonRepository<T> : IRepository<T>  where T : IHasId{
    private readonly string _filePath;
    private readonly int _option;
    public JsonRepository(string filePath, int option)
    {
        _filePath = filePath;
        _option = option;
    }

    public IMyCollection<T>? Load()
    {
        string json = File.ReadAllText(_filePath);

        var tasks = JsonSerializer.Deserialize<List<T>>(json);

        Console.WriteLine(_filePath + " " + _option);
        Console.WriteLine(File.Exists(_filePath));

        if (File.Exists(_filePath)) 
        {
            switch (_option)
            {
                case 1:
                    MyArray<T> array = [.. tasks!];
                    return array;
                case 2:
                    MyLinkedList<T> linkedList = [.. tasks!];
                    return linkedList;
                case 3:
                    MyHashMap<T> hashMap = [.. tasks!];
                    return hashMap;
                case 4:
                    MyBinarySearchTree<T> binarySearchTree = new MyBinarySearchTree<T>(default, (T a, T b) =>
                    {
                        if (a.Id > b.Id) return 1;
                        if (a.Id < b.Id) return -1;
                        return 0;                     
                    });
                    foreach (var item in tasks!)
                    {
                        binarySearchTree.Add(item);
                    }
                    return binarySearchTree;
            }       
        };
        throw new Exception(); 
    }

    public void Save(IMyCollection<T> entries) {
        string json = JsonSerializer.Serialize(entries, new
        JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}