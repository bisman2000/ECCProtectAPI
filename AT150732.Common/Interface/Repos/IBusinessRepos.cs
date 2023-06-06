

namespace AT150732.Common.Interface.Repos;

public interface IBusinessRepos<T> : IBaseRepos<T>, IReaderRepos<T>, IWriterRepos<T> where T : class
{
}
