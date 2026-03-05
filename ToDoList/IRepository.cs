public interface IRepository<T>
{
    List<T> Load();
    void Save(List<T> entries);
}