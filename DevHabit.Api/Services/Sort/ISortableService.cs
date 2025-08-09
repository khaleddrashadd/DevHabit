namespace DevHabit.Api.Services.Sort;

public interface ISortableService<T> where T : class
{
    IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy);
}