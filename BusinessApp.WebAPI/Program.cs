
using BusinessApp.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=buero";
BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>()
    .UseNpgsql(connection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

builder.Services.AddDbContext<BueroContext>(c => {
    c.UseNpgsql(connection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetService<BueroContext>())
        {
            if (db is null)
                throw new Exception("No DB!");
            //New DB!!!
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            await db.SeedBogusAsync();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
