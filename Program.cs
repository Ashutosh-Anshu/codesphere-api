using codesphere_api.Extensions;
using codesphere_api.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();
builder.Services.AddApplicationRepositories();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CodeSpherePolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});



// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("CodeSpherePolicy");
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Json(new
{
    message = "CodeSphere API is running!",
    status = "success"
}));
app.MapHealthChecks("/health");

app.Run();
