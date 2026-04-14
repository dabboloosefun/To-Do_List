public interface IMyIndexable<T> : IMyCollection<T>
{
    public T this[int index] { get; set; }
}