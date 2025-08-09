using System.Linq.Expressions;
using DevHabit.Api.Entities;

namespace DevHabit.Api.Services.Sort.Habits;

public class SortHabitService : SortableService<Habit>
{
    public SortHabitService()
    {
        ConfigureSortExpressions();
    }

    private void ConfigureSortExpressions()
    {
        AddSortExpression("name", h => h.Name);
        AddSortExpression("description", h => h.Description ?? string.Empty);
        AddSortExpression("type", h => h.Type);
        AddSortExpression("status", h => h.Status);
        AddSortExpression("frequency.type", h => h.Frequency.Type);
    }
}