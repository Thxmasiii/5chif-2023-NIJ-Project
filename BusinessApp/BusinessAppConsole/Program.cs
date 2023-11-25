// See https://aka.ms/new-console-template for more information
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

//BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>().UseSqlite($@"DataSource=..\..\..\BueroDB.db").Options);

string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=buero";
BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>()
    .UseNpgsql(connection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

//Postgres Create
async Task<long> CreateAndInsertPostgresTimer(int anz)
{
    Stopwatch timer = new();
    timer.Start();

    bueroContext.Database.EnsureDeleted();
    bueroContext.Database.EnsureCreated();
    await bueroContext.SeedBogusAsync(anz);

    timer.Stop();
    return timer.ElapsedMilliseconds;
}


//Postgres Read
long ReadPostgresTimer()
{
    Stopwatch timer = new();
    timer.Start();

     var personen = bueroContext.Personen.ToList();
     var geraete = bueroContext.Geraete.ToList();

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Postgres Update
long UpdatePostgresTimer()
{
    Stopwatch timer = new();
    timer.Start();

    var personen = bueroContext.Personen.ToList();
    var geraete = bueroContext.Geraete.ToList();
    foreach (var person in personen)
    {
        person.Name = person.Name + "Test";
    }
    foreach (var geraet in geraete)
    {
        geraet.Name = geraet.Name + "Test";
    }
    //SaveChangesAsync(); //muss man in ein anderes File auslagern.

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Postgres Delete
long DeletePostgresTimer()
{
    Stopwatch timer = new();
    timer.Start();

    var personen = bueroContext.Personen.ToList();
    var geraete = bueroContext.Geraete.ToList();

    bueroContext.Personen.RemoveRange(personen);
    bueroContext.Geraete.RemoveRange(geraete);

    //SaveChangesAsync(); //muss man in ein anderes File auslagern.

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

ConsoleTable table = new("", "SQL", "Mongo");
table.AddRow("CREATE", await CreateAndInsertPostgresTimer(100) + "ms", "" + "ms");
table.AddRow("READ",  ReadPostgresTimer() + "ms", "" + "ms");
table.AddRow("UPDATE", UpdatePostgresTimer() + "ms", "" + "ms");
table.AddRow("DELETE", DeletePostgresTimer() + "ms", "" + "ms");

Console.WriteLine(table);
