public interface IRepository<T>
{
    IEnumerable<T> Load();
    void Save(IEnumerable<T> entries);
}