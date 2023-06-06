
using Microsoft.EntityFrameworkCore;

namespace AT150732.Common.Interface;

public interface IBaseRepos<T> where T : class
{
    DbSet<T> DbSet { get; }
}
