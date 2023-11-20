// See https://aka.ms/new-console-template for more information
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

//BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>().UseSqlite($@"DataSource=..\..\..\BueroDB.db").Options);

string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=buero";
BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>()
    .UseNpgsql(connection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

bueroContext.Database.EnsureDeleted();
bueroContext.Database.EnsureCreated();

await bueroContext.SeedBogusAsync(50);


//bueroContext.Personen.Add(new Person("Leo", new DateTime(2022,11,11) ,Geschlecht.Maennlich));
//bueroContext.Bueros.Add(new Buero("Spengergasse","1050"));

//bueroContext.SaveChanges();

//bueroContext.Parkplaetze.Add(new Parkplatz(12, true, 1, 1));

//bueroContext.SaveChanges();

//Console.WriteLine(bueroContext.Personen.ToList()[0].Geschlecht);

