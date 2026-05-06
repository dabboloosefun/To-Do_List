public interface IRepository<T>
{
    IMyCollection<T>? Load();
    void Save(IMyCollection<T> entries);
}