
namespace AT150732.Infrastructure.Repository;

public class BusinessRepo<T> : IBusinessRepos<T> where T : BaseEntity
{
    public DbSet<T> DbSet => ApplicationDbContext.Set<T>();
    private ApplicationDbContext ApplicationDbContext { get; }
    public BusinessRepo(ApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext = applicationDbContext;
    }

    public void Delete(T entity)
    {
        if (ApplicationDbContext.Entry(entity).State == EntityState.Detached)
            DbSet.Attach(entity);
        DbSet.Remove(entity);
    }

    public async Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = DbSet;

        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = DbSet;

        query = query.Where(entity => entity.Id == id);

        foreach (var include in includes)
            query = query.Include(include);

        return await query.SingleOrDefaultAsync();
    }
    public async Task<int> InsertAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity.Id;
    }

    public async Task SaveChangesAsync()
    {
        await ApplicationDbContext.SaveChangesAsync();
    }

    public void Update(T entity, T oldEntity)
    {
        //remove tracking the old entity with same primary key aka id
        ApplicationDbContext.Entry(oldEntity).State = EntityState.Detached;
        //set tracking to the new entity
        DbSet.Attach(entity);
        //set tracking state of this entity is Modified so when we saveChanges it will override the exist
        ApplicationDbContext.Entry(entity).State = EntityState.Modified;
    }
}
