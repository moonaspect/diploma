public interface ICache<T>
{
    Task<IList<T>> GetCachedDataAsync(Func<Task<IList<T>>> dataLoader);
    Task SaveDataAsync(IList<T> data);
}
