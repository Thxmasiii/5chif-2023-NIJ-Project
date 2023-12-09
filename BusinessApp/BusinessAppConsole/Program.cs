// See https://aka.ms/new-console-template for more information
using Bogus;
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using BusinessApp.WebApp.Services;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Diagnostics;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;
using static BusinessApp.WebApp.Services.Service;
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

BueroMongoContext bueroMongoContext = BueroMongoContext.FromConnectionString("mongodb://localhost:27017", logging: false);
bueroMongoContext.DeleteDb();

IService service = new Service(bueroContext, bueroMongoContext);

Console.WriteLine("Benchmark Test:");

int[] anzarray = { 100, 1000, 100000 };
foreach (int anz in anzarray)
{
    Console.WriteLine($"{anz}: ");
    ConsoleTable mongo = new("", "SQL", "Mongo");
    mongo.AddRow("CREATE", service.CreateAndInsertPostgresTimer(anz) + "ms", service.CreateAndInsertMongoTimer(false, anz) + "ms");
    mongo.AddRow("READ", service.ReadAllPersons() + "ms", service.ReadMongoAllMethodes() + "ms");
    mongo.AddRow("UPDATE", service.UpdatePostgresTimer() + "ms", service.UpdateMongoTimer() + "ms");
    mongo.AddRow("DELETE", service.DeletePostgresTimer() + "ms", service.DeleteMongoTimer() + "ms");
    Console.WriteLine(mongo);
}

Console.WriteLine("Vergleich ohne und mit Aggregation");
service.CreateAndInsertPostgresTimer(1000);
service.CreateAndInsertMongoTimer(false, 1000);
ConsoleTable agg = new("", "SQL", "Mongo");
agg.AddRow("ohne", service.ReadPersonsNoFilter().Item1 + "ms", service.ReadMongoPersonsNoFilter().Item1 + "ms");
agg.AddRow("mit", service.ReadPersonsWithAggregation().Item1 + "ms", service.ReadMongoPersonsWithAggregation().Item1 + "ms");
Console.WriteLine(agg);

Console.WriteLine("Vergleich der Laufzeiten beim Setzen eines Index auf die Mongo-Struktur");
ConsoleTable index = new("", "CREATE", "READ", "UPDATE", "DELETE");
index.AddRow("ohne Index", service.CreateAndInsertMongoTimer(false, 1000) + "ms", service.ReadMongoAllMethodes() + "ms", service.UpdateMongoTimer() + "ms", service.DeleteMongoTimer() + "ms");
index.AddRow("mit Index", service.CreateAndInsertMongoTimer(true, 1000) + "ms", service.ReadMongoAllMethodes() + "ms", service.UpdateMongoTimer() + "ms", service.DeleteMongoTimer() + "ms");
Console.WriteLine(index);

int min = 100;
int steps = 100;
int max = 1000;

Console.WriteLine("Maximale Differenz in der Zeit");
ConsoleTable diffTable = new("", "Anz", "SQL", "Mongo");
List <Diff> diffCreate = new();
List<Diff> diffRead = new();
List<Diff> diffUpdate = new();
List<Diff> diffDelete = new();
for (int i = min; i < max; i += steps)
{
    diffCreate.Add(new Diff(i, service.CreateAndInsertPostgresTimer(i), service.CreateAndInsertMongoTimer(false, i)));
    diffRead.Add(new Diff(i, service.ReadAllPersons(), service.ReadMongoAllMethodes()));
    diffUpdate.Add(new Diff(i, service.UpdatePostgresTimer(), service.UpdateMongoTimer()));
    diffDelete.Add(new Diff(i, service.DeletePostgresTimer(), service.DeleteMongoTimer()));
}

Diff MaxDiffCreate = diffCreate.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffRead = diffRead.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffUpdate = diffUpdate.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffDelete = diffDelete.OrderByDescending(x => x.Differenz).First();

diffTable.AddRow("Create", MaxDiffCreate.Anz, MaxDiffCreate.Sql, MaxDiffCreate.Mongo);
diffTable.AddRow("Read", MaxDiffRead.Anz, MaxDiffRead.Sql, MaxDiffRead.Mongo);
diffTable.AddRow("Update", MaxDiffUpdate.Anz, MaxDiffUpdate.Sql, MaxDiffUpdate.Mongo);
diffTable.AddRow("Delete", MaxDiffDelete.Anz, MaxDiffDelete.Sql, MaxDiffDelete.Mongo);
Console.WriteLine(diffTable);
