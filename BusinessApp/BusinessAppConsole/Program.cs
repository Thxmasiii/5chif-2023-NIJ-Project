// See https://aka.ms/new-console-template for more information
using Bogus;
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Diagnostics;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;
using static MongoDB.Driver.WriteConcern;

//Postgres
string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=buero";
BueroContext bueroContext = new BueroContext(new DbContextOptionsBuilder<BueroContext>()
    .UseNpgsql(connection)
                //.EnableSensitiveDataLogging()
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

bueroContext.Database.EnsureDeleted();
bueroContext.Database.EnsureCreated();

//Postgres Create
long CreateAndInsertPostgresTimer(int anz)
{
    Stopwatch timer = new();
    timer.Start();

    bueroContext.SeedBogus(anz);

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
    bueroContext.SaveChanges();

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

    bueroContext.SaveChanges();

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Mongo
BueroMongoContext bueroMongoContext = BueroMongoContext.FromConnectionString("mongodb://localhost:27017", logging: false);
bueroMongoContext.DeleteDb();

//Mongo Create
long CreateAndInsertMongoTimer(int anz)
{
    Stopwatch timer = new();
    timer.Start();

    bueroMongoContext.SeedBogus(anz);

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Mongo Read
long ReadMongoTimer()
{
    Stopwatch timer = new();
    timer.Start();

    var personen = bueroMongoContext.Personen.Find(x => true).ToList();
    var geraete = bueroMongoContext.Geraete.Find(x => true).ToList();

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Mongo Update
long UpdateMongoTimer()
{
    Stopwatch timer = new();
    timer.Start();

    var updatePerson = Builders<MongoPerson>.Update
    .Set(person => person.Name, "Test");

    var personen = bueroMongoContext.Personen.UpdateMany(Builders<MongoPerson>.Filter.Where(x => true), updatePerson);

    var updateGeraet = Builders<MongoGeraet>.Update
    .Set(geraet => geraet.Name, "Test");

    var geraete = bueroMongoContext.Geraete.UpdateMany(Builders<MongoGeraet>.Filter.Where(x => true), updateGeraet);

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Mongo Delete
long DeleteMongoTimer()
{
    Stopwatch timer = new();
    timer.Start();

    bueroMongoContext.Personen.DeleteMany(Builders<MongoPerson>.Filter.Where(x => true));
    bueroMongoContext.Geraete.DeleteMany(Builders<MongoGeraet>.Filter.Where(x => true));

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//int[] timearray = { 100, 1000, 10000, 100000 };
//Console.WriteLine("SQl: ");
//ConsoleTable sql = new("Count", "CREATE", "READ", "UPDATE", "DELETE");
//foreach(int time in timearray)
//{
//    sql.AddRow(time, CreateAndInsertPostgresTimer(time) + "ms", ReadPostgresTimer() + "ms", UpdatePostgresTimer() + "ms", DeletePostgresTimer() + "ms");
//}
//Console.WriteLine(sql);

//Console.WriteLine("Mongo: ");
//ConsoleTable mongo = new("Count", "CREATE", "READ", "UPDATE", "DELETE");
//foreach (int time in timearray)
//{
//    mongo.AddRow(time, CreateAndInsertMongoTimer(time) + "ms", ReadMongoTimer() + "ms", UpdateMongoTimer() + "ms", DeleteMongoTimer() + "ms");
//}
//Console.WriteLine(mongo);

int[] timearray = { 100, 1000, 10000, 100000 };
foreach (int time in timearray)
{
    Console.WriteLine($"{time}: ");
    ConsoleTable mongo = new("", "SQL", "Mongo");
    mongo.AddRow("CREATE", CreateAndInsertPostgresTimer(time) + "ms", CreateAndInsertMongoTimer(time) + "ms");
    mongo.AddRow("READ", ReadPostgresTimer() + "ms", ReadMongoTimer() + "ms");
    mongo.AddRow("UPDATE", UpdatePostgresTimer() + "ms", UpdateMongoTimer() + "ms");
    mongo.AddRow("DELETE", DeletePostgresTimer() + "ms", DeleteMongoTimer() + "ms");
    Console.WriteLine(mongo);
}
