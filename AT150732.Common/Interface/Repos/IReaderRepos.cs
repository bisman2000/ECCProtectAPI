using System.Linq.Expressions;

namespace AT150732.Common.Interface
{
    public interface IReaderRepos<T> where T : class
    {
        Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    }
}
