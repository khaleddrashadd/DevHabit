using DevHabit.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddControllers().AddErrorHandling().AddDatabase().AddApplicationsServices();

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