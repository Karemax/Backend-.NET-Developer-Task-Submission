using Serilog;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.Application;
using Project.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();

// Add Clean Architecture Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Global Error Handling & ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add Swagger Support with JWT authorization capabilities
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project & Task Management API v1");
        options.RoutePrefix = "swagger"; // Access Swagger UI at root/swagger
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
