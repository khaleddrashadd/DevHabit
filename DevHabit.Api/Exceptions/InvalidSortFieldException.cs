namespace DevHabit.Api.Exceptions;

public sealed class InvalidSortFieldException(string message) : Exception(message)
{
}