using Bogus;
using Bogus.DataSets;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessApp.Application.Infrastructure
{
    public class BueroMongoContext : DbContext
    {
        public enum Geschlecht
        {
            Weiblich, Maennlich
        }
        public record MongoPerson(string Name, DateTime Gebdat, Geschlecht Geschlecht, ObjectId Id = default);
        public record MongoGeraet(string Name, MongoPerson Person, ObjectId Id = default);

        public MongoClient Client { get; }
        public IMongoDatabase Db { get; }
        public IMongoCollection<MongoPerson> Personen => Db.GetCollection<MongoPerson>("persons");
        public IMongoCollection<MongoGeraet> Geraete => Db.GetCollection<MongoGeraet>("geraete");

        public static BueroMongoContext FromConnectionString(string connectionString, bool logging = false)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            if (logging)
            {
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        // Bei update Statements geben wir die Anweisung aus, wie wir sie in der Shell eingeben könnten.
                        if (e.Command.TryGetValue("updates", out var updateCmd))
                        {
                            var collection = e.Command.GetValue("update");
                            var isUpdateOne = updateCmd[0]["q"].AsBsonDocument.Contains("_id");
                            foreach (var cmd in updateCmd.AsBsonArray)
                            {
                                Console.WriteLine($"db.getCollection(\"{collection}\").{(isUpdateOne ? "updateOne" : "updateMany")}({updateCmd[0]["q"]}, {updateCmd[0]["u"]})");
                            }
                        }
                        // Bei aggregate Statements geben wir die Anweisung aus, wie wir sie in der Shell eingeben könnten.
                        if (e.Command.TryGetValue("aggregate", out var aggregateCmd))
                        {
                            var collection = aggregateCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").aggregate({e.Command["pipeline"]})");
                        }

                        // Bei Filter Statements geben wir die find Anweisung aus.
                        if (e.Command.TryGetValue("find", out var findCmd))
                        {
                            var collection = findCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").find({e.Command["filter"]})");
                        }
                    });
                };
            }
            var client = new MongoClient(settings);
            var db = client.GetDatabase("bueroDb");
            // LowerCase property names.
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(ignoreIfNull: true)
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
            return new BueroMongoContext(client, db);
        }

        private BueroMongoContext(MongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;
        }

        public void DeleteDb()
        {
            try
            {
                Db.DropCollection("persons");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                Db.DropCollection("geraete");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SeedBogus(int anz)
        {
            var persons = new Faker<MongoPerson>("de").CustomInstantiator(f =>
            {
                return new MongoPerson(
                    Name: f.Name.FullName(),
                    Gebdat: f.Date.Recent().ToUniversalTime(),
                    Geschlecht: f.Random.Enum<Geschlecht>(),
                    Id: ObjectId.GenerateNewId());
            })
            .Generate(anz).ToList();
            Personen.InsertMany(persons);
            //Personen.Indexes.CreateOne(
            //    new CreateIndexModel<MongoPerson>(Builders<MongoPerson>.IndexKeys.Ascending(p => p.Id),
            //    new CreateIndexOptions() { Unique = true }));

            var geraete = new Faker<MongoGeraet>("de").CustomInstantiator(f =>
            {
                return new MongoGeraet(
                    Name: f.Commerce.ProductName(),
                    Person: f.PickRandom<MongoPerson>(persons),
                    Id: ObjectId.GenerateNewId());
            })
            .Generate(anz).ToList();
            Geraete.InsertMany(geraete);
            //Geraete.Indexes.CreateOne(
            //    new CreateIndexModel<MongoPerson>(Builders<MongoPerson>.IndexKeys.Ascending(p => p.Id),
            //    new CreateIndexOptions() { Unique = true }));
        }

        public void SeedBogusIndex(int anz)
        {
            var persons = new Faker<MongoPerson>("de").CustomInstantiator(f =>
            {
                return new MongoPerson(
                    Name: f.Name.FullName(),
                    Gebdat: f.Date.Recent(10000).ToUniversalTime(),
                    Geschlecht: f.Random.Enum<Geschlecht>(),
                    Id: ObjectId.GenerateNewId());
            })
            .Generate(anz).ToList();
            Personen.InsertMany(persons);
            Personen.Indexes.CreateOne(
                new CreateIndexModel<MongoPerson>(Builders<MongoPerson>.IndexKeys.Ascending(p => p.Id)));
                //new CreateIndexOptions() { Unique = true }));

            var geraete = new Faker<MongoGeraet>("de").CustomInstantiator(f =>
            {
                return new MongoGeraet(
                    Name: f.Commerce.ProductName(),
                    Person: f.PickRandom<MongoPerson>(persons),
                    Id: ObjectId.GenerateNewId());
            })
            .Generate(anz).ToList();
            Geraete.InsertMany(geraete);
            Geraete.Indexes.CreateOne(
                new CreateIndexModel<MongoGeraet>(Builders<MongoGeraet>.IndexKeys.Ascending(g => g.Id)));
                //new CreateIndexOptions() { Unique = true }));
        }
    }
}
