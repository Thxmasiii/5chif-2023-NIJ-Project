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

    //no filter
    var personen = bueroContext.Personen.ToList();
    var geraete = bueroContext.Geraete.ToList();

    //with filter 
    var personenFilter = bueroContext.Personen.ToList().FindAll(x => x.Gebdat < DateTime.Now.AddDays(-5000));
    var geraeteFilter = bueroContext.Geraete.ToList().FindAll(x => x.Person.Equals(personen[0]));

    //with filter and projektion
    var personenFilterProjektion =
    from person in personen.AsEnumerable()
    where person.Gebdat < DateTime.Now.AddDays(-5000)
    select person.Name; 

    var geraeteFilterProjektion =
    from g in geraete.AsEnumerable()
    where g.Person.Equals(personen[0])
    select g.Name;

    //with filter, projektion, sorting
    var personenFilterProjektionSorting =
    from person in personen.AsEnumerable()
    where person.Gebdat < DateTime.Now.AddDays(-5000)
    orderby person.Name
    select person.Name;

    var geraeteFilterProjektionSorting =
    from g in geraete.AsEnumerable()
    where g.Person.Equals(personen[0])
    orderby g.Name
    select g.Name;

    //no filter aggregate 
    var personenAggregate = bueroContext.Personen.ToList().Max(x => x.Gebdat);
    var geraeteAggregate = bueroContext.Geraete.ToList().Max(x => x.Person.Gebdat);

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Postgres Update
long UpdatePostgresTimer()
{
    var personen = bueroContext.Personen.ToList();
    var geraete = bueroContext.Geraete.ToList();

    Stopwatch timer = new();
    timer.Start();
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
    var personen = bueroContext.Personen.ToList();
    var geraete = bueroContext.Geraete.ToList();
    Stopwatch timer = new();
    timer.Start();

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

    //bueroMongoContext.SeedBogus(anz);
    bueroMongoContext.SeedBogusIndex(anz);

    timer.Stop();
    return timer.ElapsedMilliseconds;
}

//Mongo Read
long ReadMongoTimer()
{
    Stopwatch timer = new();
    timer.Start();

    // no filter
    var personen = bueroMongoContext.Personen.Find(x => true).ToList();
    var geraete = bueroMongoContext.Geraete.Find(x => true).ToList();

    // filter
    var personenFilter = bueroMongoContext.Personen.Find(x => x.Gebdat < DateTime.Now.AddDays(-5000)).ToList();
    var geraeteFilter = bueroMongoContext.Geraete.Find(x => x.Person.Equals(personen[0])).ToList();

    // filter and projektion
    var personenFilterProjektion = bueroMongoContext.Personen
        .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
        .Project(x => x.Name)
        .ToList();
    var geraeteFilterProjektion = bueroMongoContext.Geraete
        .Find(x => x.Person.Equals(personen[0]))
        .Project(x => x.Name)
        .ToList();

    //with filter, projektion, sorting
    var personenFilterProjektionSorting = bueroMongoContext.Personen
        .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
        .Project(x => x.Name)
        .SortBy(x => x.Name)
        .ToList();
    var geraeteFilterProjektionSorting = bueroMongoContext.Geraete
        .Find(x => x.Person.Equals(personen[0]))
        .Project(x => x.Name)
        .SortBy(x => x.Name)
        .ToList();

    //no filter, aggregation
    var personenAggregation = bueroMongoContext.Personen.Aggregate()
                                .Group(x => x.Gebdat, g=>
                                    new {
                                        Max = g.Max(a => DateTime.Now - a.Gebdat)
                                    }).ToList()[0];
    var geraeteAggregation = bueroMongoContext.Geraete.Aggregate()
                                .Group(x => x.Person, g =>
                                    new {
                                        Max = g.Max(a => DateTime.Now - a.Person.Gebdat)
                                    }).ToList()[0];

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

int[] timearray = { 100, 1000 };
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

Console.WriteLine("Vergleich ohne und mit Aggregation");
CreateAndInsertPostgresTimer(100);
CreateAndInsertMongoTimer(100);
Console.WriteLine("SQL: ");
ConsoleTable agg = new("", "SQL", "Mongo");
agg.AddRow("ohne", ReadPostgresTimer() + "ms", ReadMongoTimer() + "ms");
agg.AddRow("mit", ReadPostgresTimer() + "ms", ReadMongoTimer() + "ms");
Console.WriteLine(agg);
