using DevHabit.Api;
using DevHabit.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddApplicationsServices()
    .AddAuthenticationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.SeedRoles();
}

app.UseHttpsRedirection();

//ther order of these middlewares is important 
app.UseAuthentication();
app.UseAuthorization();
// use caching header middleware (this after using:  builder.Services.AddResponseCaching();) 
app.UseResponseCaching();

app.UseExceptionHandler();

app.MapControllers();

app.Run();