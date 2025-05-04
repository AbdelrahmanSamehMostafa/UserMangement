using UserManagment.API.Extensions;
using UserManagment.API.Filters;
using UserManagment.Application.Extensions;
using UserManagment.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers(config =>
{
    config.Filters.Add(typeof(UserManagmentExceptionHandler)); // Add your custom exception handler filter
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// Swagger configuration should be done before building the app
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("Authentication And Authorization", new OpenApiInfo
    {
        Version = "v1",
        Title = "Authentication And Authorization",
        Description = "API documentation for Authentication and Authorization. Details endpoints for user authentication, authorization, and related security features.",
    });
    c.SwaggerDoc("System Configuration", new OpenApiInfo
    {
        Version = "v1",
        Title = "System Configuration",
        Description = "API documentation for System Configuration.",
    });

    c.SwaggerDoc("Role", new OpenApiInfo
    {
        Version = "v1",
        Title = "Roles",
        Description = "API documentation for Roles.",
    });

    c.SwaggerDoc("Group", new OpenApiInfo
    {
        Version = "v1",
        Title = "Group",
        Description = "API documentation for Group.",
    });

    c.SwaggerDoc("User Management", new OpenApiInfo
    {
        Version = "v1",
        Title = "User Management",
        Description = "API documentation for User Management.",
    });
    c.SwaggerDoc("LookUps", new OpenApiInfo
    {
        Version = "v1",
        Title = "LookUps",
        Description = "API documentation for LookUps.",
    });
    c.SwaggerDoc("Logs", new OpenApiInfo
    {
        Version = "v1",
        Title = "Logs",
        Description = "API documentation for Logs.",
    });
    c.SwaggerDoc("Screen Menu", new OpenApiInfo
    {
        Version = "v1",
        Title = "Screen Menu",
        Description = "API documentation for Screen Menu.",
    });

});
builder.InitSwagger();
builder.Services.AddHttpContextAccessor();

builder.RegisterAuthentication();
builder.Services.AddApplicationServices();
builder.Services.AddDataServices(builder.Configuration);
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Error);
builder.Host.UseNLog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/Authentication And Authorization/swagger.json", "Authentication And Authorization");
    c.SwaggerEndpoint("/swagger/System Configuration/swagger.json", "System Configuration");
    c.SwaggerEndpoint("/swagger/Role/swagger.json", "Role");
    c.SwaggerEndpoint("/swagger/Group/swagger.json", "Group");
    c.SwaggerEndpoint("/swagger/User Management/swagger.json", "User Management");
    c.SwaggerEndpoint("/swagger/LookUps/swagger.json", "LookUps");
    c.SwaggerEndpoint("/swagger/Logs/swagger.json", "Logs");
    c.SwaggerEndpoint("/swagger/Screen Menu/swagger.json", "Screen Menu");


    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
    c.DefaultModelsExpandDepth(-1);
    c.EnablePersistAuthorization(); // For Authorization Across Multiple Swagger Groups
});
app.UseMiddleware<PolicyAuthorizationMiddleware>();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();