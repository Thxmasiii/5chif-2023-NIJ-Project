using Bogus;
using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using Microsoft.EntityFrameworkCore.Query;
using MongoDB.Driver;
using System.Diagnostics;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;

namespace BusinessApp.WebApp.Services
{
    public class Service: IService
    {
        public BueroContext BueroContext { get; set; }
        public BueroMongoContext BueroMongoContext { get; set; }

        public Service(BueroContext bueroContext, BueroMongoContext bueroMongoContext)
        {
            BueroContext = bueroContext;
            BueroMongoContext = bueroMongoContext;
            Console.WriteLine(CreateAndInsertMongoTimer(true,10) + "ms ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
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

        public List<Application.Model.Person> GetPersonsNoFilter(int anz)
        {
            CreateAndInsertPostgresTimer(anz);
            Stopwatch timer = new();

            timer.Start();
            var personen = BueroContext.Personen.ToList();
            timer.Stop();

            return personen;
        }

        public List<Application.Model.Person> GetPersonsWithFilter(int anz)
        {
            CreateAndInsertPostgresTimer(anz);
            Stopwatch timer = new();

            timer.Start();
            var personen = BueroContext.Personen.ToList().FindAll(x => x.Gebdat < DateTime.Now.AddDays(-5000));
            timer.Stop();

            return personen;
        }

        //public List<Tuple<int,string>> GetPersonsWithFilterAndProjektion(int anz)
        //{
        //    CreateAndInsertPostgresTimer(anz);
        //    Stopwatch timer = new();

        //    timer.Start();
        //    var query = (from person in personen.AsEnumerable()
        //                where person.Gebdat < DateTime.Now.AddDays(-5000)
        //                select new { Id = person.Id, Name = person.Name });<
        //    var personen = query.ToList<Tuple<int, string>>();
            
        //    timer.Stop();

        //    return personen;
        //}

        //Postgres Read
        public (List<Person>,long) ReadPostgresTimer(int anz, int filter) // 0 = no filter, 1 = filter, 2 = filter and projection, 3 = filter, projection and sorting, 4 = no filter aggregate
        {
            CreateAndInsertPostgresTimer(anz);
            Stopwatch timer = new();
            timer.Start();

            //no filter
            var personen = BueroContext.Personen.ToList();
            var geraete = BueroContext.Geraete.ToList();
            //with filter 
            if (filter == 1)
            {
                personen = BueroContext.Personen.ToList().FindAll(x => x.Gebdat < DateTime.Now.AddDays(-5000));
                geraete = BueroContext.Geraete.ToList().FindAll(x => x.Person.Equals(personen[0]));
            }
            //with filter and projektion
            if (filter == 2)
            {
                var query = from person in personen
                            where person.Gebdat < DateTime.Now.AddDays(-5000)
                            select new { Id = person.Id, Name = person.Name };
                //personen = query.ToList<(int, string)> ();
                //geraete =
                //    from g in geraete.AsEnumerable()
                //    where g.Person.Equals(personen[0])
                //    select g.Name;
            }


            //with filter, projektion, sorting
            if (filter == 3)
            {
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
            }

            //no filter aggregate 
            if (filter == 4)
            {
                var personenAggregate = BueroContext.Personen.ToList().Max(x => x.Gebdat);
                var geraeteAggregate = BueroContext.Geraete.ToList().Max(x => x.Person.Gebdat);
            }
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        public List<Geraet> GetGeraetePerPerson(int id)
        {
            return BueroContext.Personen.ToList().FirstOrDefault(x => x.Id == id).Geraete.ToList();
        }

        //Postgres Update
        public long UpdatePostgresTimer(int anz)
        {
            CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.ToList();
            var geraete = BueroContext.Geraete.ToList();

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
            CreateAndInsertPostgresTimer(anz);
            var personen = BueroContext.Personen.ToList();
            var geraete = BueroContext.Geraete.ToList();
            Stopwatch timer = new();
            timer.Start();

            BueroContext.Personen.RemoveRange(personen);
            BueroContext.Geraete.RemoveRange(geraete);

            BueroContext.SaveChanges();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo
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
        public long ReadMongoTimer(bool withIndex, int anz, int filter) // 0 = no filter, 1 = filter, 2 = filter and projection, 3 = filter, projection and sorting, 4 = no filter aggregate
        {
            CreateAndInsertMongoTimer(withIndex, anz);
            Stopwatch timer = new();
            timer.Start();

            // no filter
            var personen = BueroMongoContext.Personen.Find(x => true).ToList();
            var geraete = BueroMongoContext.Geraete.Find(x => true).ToList();

            // filter
            if (filter == 1)
            {
                var personenFilter = BueroMongoContext.Personen.Find(x => x.Gebdat < DateTime.Now.AddDays(-5000)).ToList();
                var geraeteFilter = BueroMongoContext.Geraete.Find(x => x.Person.Equals(personen[0])).ToList();
            }

            // filter and projektion
            if (filter == 2)
            {
                var personenFilterProjektion = BueroMongoContext.Personen
                    .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
                    .Project(x => x.Name)
                    .ToList();
                var geraeteFilterProjektion = BueroMongoContext.Geraete
                    .Find(x => x.Person.Equals(personen[0]))
                    .Project(x => x.Name)
                    .ToList();
            }
            //with filter, projektion, sorting
            if (filter == 3)
            {
                var personenFilterProjektionSorting = BueroMongoContext.Personen
                    .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
                    .Project(x => x.Name)
                    .SortBy(x => x.Name)
                    .ToList();
                var geraeteFilterProjektionSorting = BueroMongoContext.Geraete
                    .Find(x => x.Person.Equals(personen[0]))
                    .Project(x => x.Name)
                    .SortBy(x => x.Name)
                    .ToList();
            }

            //no filter, aggregation
            if (filter == 4)
            {
                var personenAggregation = BueroMongoContext.Personen.Aggregate()
                                            .Group(x => x.Gebdat, g =>
                                                new
                                                {
                                                    Max = g.Max(a => DateTime.Now - a.Gebdat)
                                                }).ToList()[0];
                var geraeteAggregation = BueroMongoContext.Geraete.Aggregate()
                                            .Group(x => x.Person, g =>
                                                new
                                                {
                                                    Max = g.Max(a => DateTime.Now - a.Person.Gebdat)
                                                }).ToList()[0];
            }
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        //Mongo Update
        public long UpdateMongoTimer(bool withIndex, int anz)
        {
            CreateAndInsertMongoTimer(withIndex, anz);
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
            CreateAndInsertMongoTimer(withIndex, anz);
            Stopwatch timer = new();
            timer.Start();

            BueroMongoContext.Personen.DeleteMany(Builders<MongoPerson>.Filter.Where(x => true));
            BueroMongoContext.Geraete.DeleteMany(Builders<MongoGeraet>.Filter.Where(x => true));

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
    }
}
