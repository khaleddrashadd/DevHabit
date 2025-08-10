using DevHabit.Api.Database;
using DevHabit.Api.Entities;
using DevHabit.Api.Middleware;
using DevHabit.Api.Services.Sort;
using DevHabit.Api.Services.Sort.Habits;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevHabit.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddNewtonsoftJson(); // AddNewtonsoftJson to support patch requestss
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        // it must be that global exception be the last one added
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<InvalidSortFieldExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevHabitConnectionString"),
                sqlserverOption =>
                    sqlserverOption.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
        );
        return builder;
    }

    public static WebApplicationBuilder AddApplicationsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddSingleton<ISortableService<Habit>, SortHabitService>();


        builder.Services.AddSwaggerGen();
        return builder;
    }
}