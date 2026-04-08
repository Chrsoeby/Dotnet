using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Npgsql; // Add this using statement at the top

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// 2. Configure Database Connection
// Check if we have a PostgreSQL connection string (from Render)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("Host="))
{
    // We are in the cloud (Render) - Use PostgreSQL
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // We are on your Mac (Local) - Use SQLite (Fallback)
    // This lets you keep testing locally without installing Postgres yet!
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=BookTracker.db"));
}

// 3. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://*.onrender.com") // Allow Render frontend later
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 4. Ensure Database is Created (Works for both SQLite and Postgres)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 5. Pipeline
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();