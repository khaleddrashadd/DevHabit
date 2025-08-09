namespace DevHabit.Api.Services.Sorting;

public interface ISortableService<T> where T : class
{
    IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy);
}