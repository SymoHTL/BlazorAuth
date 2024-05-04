namespace Domain.Repositories;

public class Repository<TEntity> where TEntity : class {
    protected ApplicationDbContext Context;
    protected DbSet<TEntity> Table;

    public Repository(ApplicationDbContext context) {
        Context = context;
        Table = context.Set<TEntity>();
    }
    
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> func, CancellationToken ct,
        bool tracking = false, bool ignoreAutoIncludes = false,
        params (Expression<Func<TEntity, object>>, SortDirection)[] sort) {
        var query = Table.AsQueryable();
        if (ignoreAutoIncludes) query = query.IgnoreAutoIncludes();
        if (!tracking) query = query.AsNoTracking();
        return await query
            .OrderByMultiple(sort)
            .FirstOrDefaultAsync(func, ct);
    }

    public async Task<TEntity?> FirstOrDefaultAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, TKey key,
        CancellationToken ct,
        bool tracking = false, bool ignoreAutoIncludes = false,
        params (Expression<Func<TEntity, object>>, SortDirection)[] sort) {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var body = Expression.Equal(Expression.Invoke(keySelector, parameter), Expression.Constant(key, typeof(TKey)));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await FirstOrDefaultAsync(lambda, ct, tracking, ignoreAutoIncludes, sort);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(string property, object value, CancellationToken ct,
        bool tracking = false, bool ignoreAutoIncludes = false,
        params (Expression<Func<TEntity, object>>, SortDirection)[] sort) {
        var query = Table.AsQueryable();
        if (ignoreAutoIncludes) query = query.IgnoreAutoIncludes();
        if (!tracking) query = query.AsNoTracking();
        return await query
            .OrderByMultiple(sort)
            .FirstOrDefaultAsync(t => EF.Property<object>(t, property) == value, ct);
    }
    
    public void AddAsync(params TEntity[] entity) => Table.AddRangeAsync(entity);
    
    public void Update(params TEntity[] entity) => Table.UpdateRange(entity);
    
    public void Remove(params TEntity[] entity) => Table.RemoveRange(entity);
    
    public async Task<int> SaveAsync(CancellationToken ct) => await Context.SaveChangesAsync(ct);
    
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) =>
        await Table.IgnoreAutoIncludes().AsNoTracking()
            .AnyAsync(filter, ct);

    public async Task<int> CountAsync<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector,
        string? search, CancellationToken ct) {
        return await Search(propertySelector, search, 0, int.MaxValue)
            .AsNoTracking().IgnoreAutoIncludes()
            .CountAsync(cancellationToken: ct);
    }
    
    public async Task<int> CountAsync(string property, string? search, CancellationToken ct) {
        return await Search(property, search, 0, int.MaxValue)
            .AsNoTracking().IgnoreAutoIncludes()
            .CountAsync(cancellationToken: ct);
    }

    public IQueryable<TEntity> Search<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector,
        string? search, int skip, int take,
        params (Expression<Func<TEntity, object>>, SortDirection)[] sort) {
        if (skip < 0 || take <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(skip)} or {nameof(take)} is less than or equal to 0");

        var query = Table.AsQueryable();
        if (!search.IsNullEmptyOrWhiteSpace()) query = SearchFor(propertySelector, search!);
        return  query
            .OrderByMultiple(sort)
            .SkipTake(skip, take);
    }

    protected IQueryable<TEntity> SearchFor<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector,
        string search) =>
        Table.AsNoTracking().Where(CreateLikeExpression(propertySelector, search));

    private static Expression<Func<TEntity, bool>> CreateLikeExpression<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelector, string search) {
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)]);
        var likeCall = Expression.Call(likeMethod ?? throw new InvalidOperationException(),
            Expression.Constant(EF.Functions), propertySelector.Body, Expression.Constant($"%{search}%"));

        return Expression.Lambda<Func<TEntity, bool>>(likeCall, propertySelector.Parameters);
    }

    public IQueryable<TEntity> Search(string property,
        string? search, int skip, int take, params (Expression<Func<TEntity, object>>, SortDirection)[] sort) {
        if (skip < 0 || take <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(skip)} or {nameof(take)} is less than or equal to 0");

        var query = Table.AsQueryable();
        if (!search.IsNullEmptyOrWhiteSpace()) query = SearchFor(property, search!);
        return  query
            .OrderByMultiple(sort)
            .SkipTake(skip, take);
    }

    protected IQueryable<TEntity> SearchFor(string property, string search) =>
        Table.AsNoTracking().Where(CreateLikeExpression(property, search));

    private static Expression<Func<TEntity, bool>> CreateLikeExpression(string property, string search) {
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)]);
    
        var parameterExp = Expression.Parameter(typeof(TEntity), "e");
        var propertyExp = Expression.Property(parameterExp, property);
        var likeCall = Expression.Call(
            likeMethod ?? throw new InvalidOperationException("Like method not found."),
            Expression.Constant(EF.Functions),
            propertyExp,
            Expression.Constant($"%{search}%"));

        return Expression.Lambda<Func<TEntity, bool>>(likeCall, parameterExp);
    }

}