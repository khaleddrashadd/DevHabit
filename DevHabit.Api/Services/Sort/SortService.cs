using System.Linq.Expressions;

namespace DevHabit.Api.Services.Sort;

public class SortableService<T> : ISortableService<T> where T : class
{
    private readonly Dictionary<string, Expression<Func<T, object>>> _sortExpressions = new(
        StringComparer.OrdinalIgnoreCase);

    protected void AddSortExpression(string key, Expression<Func<T, object>> expression)
    {
        _sortExpressions[key] = expression;
    }

    private static List<(string fieldName, bool isDesc)> ParseSortString(string sortBy)
    {
        var result = sortBy.Split(',').Select(str => str.Trim()).Select(str =>
        {
            var parts = str.Split(' ');
            var fieldName = parts[0];
            var isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            return (fieldName, isDesc);
        }).ToList();
        return result;
    }

    public IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        //
        var sortFields = ParseSortString(sortBy);
        IOrderedQueryable<T>? orderedQuery = null;
        foreach (var (fieldName, isDesc, isFirst) in sortFields.Select((field, idx) =>
                     (field.fieldName, field.isDesc, isFirst: idx == 0)))
        {
            //check if the fieldName exists in the 
            //sort expressions are filled by the derived class
            if (!_sortExpressions.TryGetValue(fieldName, out var expression)) continue;
            if (isFirst)
                orderedQuery = isDesc
                    ? query.OrderByDescending(expression)
                    : query.OrderBy(expression);
            else
                orderedQuery = isDesc
                    ? orderedQuery?.ThenByDescending(expression)
                    : orderedQuery?.ThenBy(expression);
        }

        return orderedQuery ?? query;
    }
}