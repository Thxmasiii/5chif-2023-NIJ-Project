using Bogus;
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Person = BusinessApp.Application.Model.Person;

namespace BusinessApp.WebApp.Services
{
    public class Service : IService
    {
        public BueroContext BueroContext { get; set; }
        public BueroMongoContext BueroMongoContext { get; set; }

        public Service(BueroContext bueroContext, BueroMongoContext bueroMongoContext)
        {
            BueroContext = bueroContext;
            BueroMongoContext = bueroMongoContext;
            //CreateAndInsertPostgresTimer(1);
            //CreateAndInsertMongoTimer(true, 1);
        }

        //Postgres Create
        public long CreateAndInsertPostgresTimer(int anz)
        {
            BueroContext.Database.EnsureDeleted();
            BueroContext.Database.EnsureCreated();
            Stopwatch timer = new();
            timer.Start();

            BueroContext.SeedBogus(anz);

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Postgres Read
        public (long, List<Person>) ReadPersonsNoFilter(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            Stopwatch timer = new();

            timer.Start();
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            timer.Stop();

            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<Person>) ReadPersonsWithFilter(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            Stopwatch timer = new();

            timer.Start();
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList().FindAll(x => x.Gebdat < DateTime.Now.AddDays(-5000));
            timer.Stop();

            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<Person>) ReadPersonsWithFilterAndProjektion(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            Stopwatch timer = new();

            timer.Start();
            var query = (from person in personen.AsEnumerable()
                         where person.Gebdat < DateTime.Now.AddDays(-5000)
                         select new { person.Id, person.Name });
            Console.WriteLine("query: " + query.Count());
            timer.Stop();

            personen = new();

            query.ToList().ForEach(person =>
            {
                Person p = new Person();
                p.Id = person.Id;
                p.Name = person.Name;
                personen.Add(p);
            });
            Console.WriteLine("personen: " + personen.Count());

            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<Person>) ReadPersonsWithFilterProjektionAndSorting(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            Stopwatch timer = new();
            timer.Start();

            var personenFilterProjektionSorting =
            (from person in personen.AsEnumerable()
             where person.Gebdat < DateTime.Now.AddDays(-5000)
             orderby person.Name
             select new { person.Id, person.Name });

            timer.Stop();

            personen = new();

            personenFilterProjektionSorting.ToList().ForEach(person =>
            {
                Person p = new Person();
                p.Id = person.Id;
                p.Name = person.Name;
                personen.Add(p);
            });

            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, DateTime) ReadPersonsWithAggregation(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            Stopwatch timer = new();
            timer.Start();

            var personenAggregate = BueroContext.Personen.ToList().Max(x => x.Gebdat);

            timer.Stop();
            return (timer.ElapsedMilliseconds, personenAggregate);
        }

        public List<Geraet> GetGeraetePerPerson(int id)
        {
            Person p = BueroContext.Personen.Include(x => x.Geraete).ToList().FirstOrDefault(x => x.Id == id);
            if (p == null)
                return new List<Geraet>();
            else
                return p.Geraete.ToList();

        }

        //Postgres Update
        public long UpdatePostgresTimer(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            var geraete = BueroContext.Geraete.Include(x => x.Person).ToList();

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
            BueroContext.SaveChanges();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Postgres Delete
        public long DeletePostgresTimer(int anz)
        {
            //CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.Include(x => x.Geraete).ToList();
            var geraete = BueroContext.Geraete.Include(x => x.Person).ToList();
            Stopwatch timer = new();
            timer.Start();

            BueroContext.Personen.RemoveRange(personen);
            BueroContext.Geraete.RemoveRange(geraete);

            BueroContext.SaveChanges();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Postgres Geraet Add
        public long AddGeraetPostgresTimer(Geraet geraet)
        {
            Stopwatch timer = new();
            timer.Start();
            BueroContext.Geraete.Add(geraet);
            BueroContext.SaveChanges();
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo
        public List<MongoGeraet> GetGeraetePerMongoPerson(string id)
        {
            if (id == null)
                return new List<MongoGeraet>();
            else
            {
               return BueroMongoContext.Geraete.Find(x => x.Person.Id == new ObjectId(id)).ToList();
            }
        }
        //Mongo Create
        public long CreateAndInsertMongoTimer(bool withIndex, int anz)
        {
            BueroMongoContext.DeleteDb();
            Stopwatch timer = new();
            timer.Start();

            if (withIndex)
                BueroMongoContext.SeedBogusIndex(anz);
            else
                BueroMongoContext.SeedBogus(anz);

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo Read
        public (long, List<MongoPerson>) ReadMongoPersonsNoFilter(int anz)
        {
            //CreateAndInsertMongoTimer(false, anz);
            Stopwatch timer = new();
            timer.Start();
            List<MongoPerson> personen = BueroMongoContext.Personen.Find(x => true).ToList();
            timer.Stop();
            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<MongoPerson>) ReadMongoPersonsWithFilter(int anz)
        {
            //CreateAndInsertMongoTimer(false, anz);
            Stopwatch timer = new();
            timer.Start();
            List<MongoPerson> personen = BueroMongoContext.Personen.Find(x => x.Gebdat < DateTime.Now.AddDays(-5000)).ToList();
            timer.Stop();
            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<MongoPerson>) ReadMongoPersonsWithFilterAndProjection(int anz)
        {
            //CreateAndInsertMongoTimer(false, anz);
            List<MongoPerson> personen = new();
            Stopwatch timer = new();
            timer.Start();
            var personenFilterProjektion = BueroMongoContext.Personen
                .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
                .Project(x => new { x.Id, x.Name })
                .ToList();
            timer.Stop();
            personenFilterProjektion.ToList().ForEach(person =>
            {
                MongoPerson p = new(person.Name, DateTime.UtcNow, Application.Infrastructure.BueroMongoContext.Geschlecht.Maennlich, person.Id);
                personen.Add(p);
            });
            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, List<MongoPerson>) ReadMongoTimerWithFilterProjektionAndSorting(int anz)
        {
            //CreateAndInsertMongoTimer(false, anz);
            List<MongoPerson> personen = new();
            Stopwatch timer = new();
            timer.Start();
            var personenFilterProjektionSorting = BueroMongoContext.Personen
                .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
                .Project(x => new { x.Id, x.Name })
                .SortBy(x => x.Name)
                .ToList();
            timer.Stop();
            personenFilterProjektionSorting.ToList().ForEach(person =>
            {
                MongoPerson p = new(person.Name, DateTime.UtcNow, Application.Infrastructure.BueroMongoContext.Geschlecht.Maennlich, person.Id);
                personen.Add(p);
            });
            return (timer.ElapsedMilliseconds, personen);
        }

        public (long, DateTime) ReadMongoPersonsWithAggregation(int anz)
        {
            //CreateAndInsertMongoTimer(false, anz);
            Stopwatch timer = new();
            timer.Start();
            var personenAggregation = BueroMongoContext.Personen.Aggregate()
                                        .Group(x => x.Gebdat, g =>
                                                                                   new
                                                                                   {
                                                                                       Gebdat = g.Max(a => a.Gebdat)
                                                                                   }).ToList()[0];
            timer.Stop();
            return (timer.ElapsedMilliseconds, personenAggregation.Gebdat);
        }

        //Mongo Update
        public long UpdateMongoTimer(bool withIndex, int anz)
        {
            //CreateAndInsertMongoTimer(withIndex, anz);
            Stopwatch timer = new();
            timer.Start();

            var updatePerson = Builders<MongoPerson>.Update
            .Set(person => person.Name, "Test");

            var personen = BueroMongoContext.Personen.UpdateMany(Builders<MongoPerson>.Filter.Where(x => true), updatePerson);

            var updateGeraet = Builders<MongoGeraet>.Update
            .Set(geraet => geraet.Name, "Test");

            var geraete = BueroMongoContext.Geraete.UpdateMany(Builders<MongoGeraet>.Filter.Where(x => true), updateGeraet);

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo Delete
        public long DeleteMongoTimer(bool withIndex, int anz)
        {
            //CreateAndInsertMongoTimer(withIndex, anz);
            Stopwatch timer = new();
            timer.Start();

            BueroMongoContext.Personen.DeleteMany(Builders<MongoPerson>.Filter.Where(x => true));
            BueroMongoContext.Geraete.DeleteMany(Builders<MongoGeraet>.Filter.Where(x => true));

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo Geraet Add
        public long AddGeraetMongoTimer(MongoGeraet geraet)
        {
            Stopwatch timer = new();
            timer.Start();
            BueroMongoContext.Geraete.InsertOne(geraet);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
    }
}
