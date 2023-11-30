using BusinessApp.Application.Infrastructure;
using BusinessApp.WebApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using ParkBee.MongoDb;
using ParkBee.MongoDb.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IService, Service>();

string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=buero";
BueroContext BueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>()
    .UseNpgsql(connection)
                //.EnableSensitiveDataLogging()
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

builder.Services.AddDbContext<BueroContext>(c =>
{
    c.UseNpgsql(connection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
}
);

BueroMongoContext BueroMongoContext = BueroMongoContext.FromConnectionString("mongodb://localhost:27017", logging: false);
BueroMongoContext.DeleteDb();

builder.Services.AddTransient(provider => BueroMongoContext);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    BueroContext.Database.EnsureDeleted();
    BueroContext.Database.EnsureCreated();
    BueroMongoContext.DeleteDb();
    Service s = new Service(BueroContext, BueroMongoContext);
    s.CreateAndInsertPostgresTimer(10);
    s.CreateAndInsertMongoTimer(true, 10);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
