
namespace AT150732.Common.Interface;

public interface IWriterRepos<T> where T : class
{
    Task<int> InsertAsync(T entity);
    void Update(T entity, T oldEntity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
