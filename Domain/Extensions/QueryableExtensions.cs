namespace Domain.Extensions;

public static class QueryableExtensions {
    public static IQueryable<T> OrderByMultiple<T, TKey>(this IQueryable<T> query,
        (Expression<Func<T, TKey>> prop, SortDirection direction)[] sort) {
        if (sort.Length == 0)
            return query;

        var ordered = query.OrderByDirection(sort[0].prop, sort[0].direction);

        if (sort.Length <= 1) return ordered;

        for (var i = 1; i < sort.Length; i++)
            ordered = ordered.ThenByDirection(sort[i].prop, sort[i].direction);

        return ordered;
    }

    public static IQueryable<T> OrderByMultiple<T, TKey>(this IQueryable<T> query,
        (string prop, SortDirection direction)[] sort) {
        if (sort.Length == 0)
            return query;
        
        var ordered = query.OrderByDirection(sort[0].prop, sort[0].direction);
        
        if (sort.Length <= 1) return ordered;
        
        for (var i = 1; i < sort.Length; i++)
            ordered = ordered.ThenByDirection<T, TKey>(sort[i].prop, sort[i].direction);
        
        return ordered;
    }

    public static IQueryable<T> SkipTake<T>(this IQueryable<T> query, int skip, int take) =>
        query.Skip(skip).Take(take);

    public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector, SortDirection direction) =>
        direction switch {
            SortDirection.Descending => source.OrderByDescending(keySelector),
            SortDirection.Ascending => source.OrderBy(keySelector),
            _ => (IOrderedQueryable<TSource>)source
        };

    public static IOrderedQueryable<TSource> ThenByDirection<TSource, TKey>(this IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector, SortDirection direction) =>
        direction switch {
            SortDirection.Descending => source.ThenByDescending(keySelector),
            SortDirection.Ascending => source.ThenBy(keySelector),
            _ => source
        };


    public static IOrderedQueryable<TEntity> OrderByDirection<TEntity>(this IQueryable<TEntity> source, string propertyName,
        SortDirection direction) =>
        OrderByDirection<TEntity, TEntity>(source, e => EF.Property<TEntity>(e!, propertyName), direction);

    public static IOrderedQueryable<TSource> ThenByDirection<TSource, TKey>(this IOrderedQueryable<TSource> source, string propertyName,
        SortDirection direction) =>
        ThenByDirection<TSource, TKey>(source, e => EF.Property<TKey>(e!, propertyName), direction);
}