using DevHabit.Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevHabitConnectionString"),
        sqlserverOption =>
            sqlserverOption.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
);

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();