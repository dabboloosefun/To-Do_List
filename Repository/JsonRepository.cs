using System.Text.Json;

class JsonRepository<T> : IRepository<T> {
    private readonly string _filePath;
    public JsonRepository(string filePath) => _filePath =
    filePath;
    public List<T> Load() {
        if (!File.Exists(_filePath)) {
            return new List<T>();
        }
        string json = File.ReadAllText(_filePath);
        var tasks = JsonSerializer.Deserialize<List<T>>(json);
        return tasks ?? new List<T>();
    }
    public void Save(List<T> entries) {
        string json = JsonSerializer.Serialize(entries, new
        JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
} 