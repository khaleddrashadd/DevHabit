using DevHabit.Api.Database;
using DevHabit.Api.Middleware;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

//config to use the slugify parameter transformer to make it like /api/habits/{id} not /api/Habits/{Id}

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers().AddNewtonsoftJson(); // AddNewtonsoftJson to support patch requestss
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevHabitConnectionString"),
        sqlserverOption =>
            sqlserverOption.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
);
// it must be that global exception be the last one added
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();

app.Run();