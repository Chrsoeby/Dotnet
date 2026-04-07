using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// 2. Configure SQLite Database Connection
// This creates a file called 'BookTracker.db' in the Api folder
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=BookTracker.db"));

// 3. Configure CORS (Keep your existing policy)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 4. Apply Migrations automatically on startup (Optional but helpful for dev)
// This ensures the database exists and has the correct structure
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 5. Configure the HTTP request pipeline.
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();