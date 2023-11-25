//using BusinessApp.Application.Infrastructure;
//using System.Diagnostics;

//namespace BusinessApp.WebApp.Services
//{
//    public class Service
//    {
//        public BueroContext BueroContext { get; set; }
//        public BueroMongoContext BueroMongoContext { get; set; }

//        public Service(BueroContext bueroContext, BueroMongoContext bueroMongoContext)
//        {
//            BueroContext = bueroContext;
//            BueroMongoContext = bueroMongoContext;
//        }

//        //Postgres Create
//        public long CreateAndInsertPostgresTimer(int anz)
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            BueroContext.SeedBogus(anz);

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Postgres Read
//        long ReadPostgresTimer()
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            //no filter
//            var personen = BueroContext.Personen.ToList();
//            var geraete = BueroContext.Geraete.ToList();

//            //with filter 
//            var personenFilter = BueroContext.Personen.ToList().FindAll(x => x.Gebdat < DateTime.Now.AddDays(-5000));
//            var geraeteFilter = BueroContext.Geraete.ToList().FindAll(x => x.Person.Equals(personen[0]));

//            //with filter and projektion
//            var personenFilterProjektion =
//            from person in personen.AsEnumerable()
//            where person.Gebdat < DateTime.Now.AddDays(-5000)
//            select person.Name;

//            var geraeteFilterProjektion =
//            from g in geraete.AsEnumerable()
//            where g.Person.Equals(personen[0])
//            select g.Name;

//            //with filter, projektion, sorting
//            var personenFilterProjektionSorting =
//            from person in personen.AsEnumerable()
//            where person.Gebdat < DateTime.Now.AddDays(-5000)
//            orderby person.Name
//            select person.Name;

//            var geraeteFilterProjektionSorting =
//            from g in geraete.AsEnumerable()
//            where g.Person.Equals(personen[0])
//            orderby g.Name
//            select g.Name;

//            //no filter aggregate 
//            var personenAggregate = BueroContext.Personen.ToList().Max(x => x.Gebdat);
//            var geraeteAggregate = BueroContext.Geraete.ToList().Max(x => x.Person.Gebdat);

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Postgres Update
//        long UpdatePostgresTimer()
//        {
//            var personen = BueroContext.Personen.ToList();
//            var geraete = BueroContext.Geraete.ToList();

//            Stopwatch timer = new();
//            timer.Start();
//            foreach (var person in personen)
//            {
//                person.Name = person.Name + "Test";
//            }
//            foreach (var geraet in geraete)
//            {
//                geraet.Name = geraet.Name + "Test";
//            }
//            BueroContext.SaveChanges();

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Postgres Delete
//        long DeletePostgresTimer()
//        {
//            var personen = BueroContext.Personen.ToList();
//            var geraete = BueroContext.Geraete.ToList();
//            Stopwatch timer = new();
//            timer.Start();

//            BueroContext.Personen.RemoveRange(personen);
//            BueroContext.Geraete.RemoveRange(geraete);

//            BueroContext.SaveChanges();

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Mongo
//        BueroMongoContext BueroMongoContext = BueroMongoContext.FromConnectionString("mongodb://localhost:27017", logging: false);
//        BueroMongoContext.DeleteDb();

////Mongo Create
//long CreateAndInsertMongoTimer(int anz)
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            //BueroMongoContext.SeedBogus(anz);
//            BueroMongoContext.SeedBogusIndex(anz);

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Mongo Read
//        long ReadMongoTimer()
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            // no filter
//            var personen = BueroMongoContext.Personen.Find(x => true).ToList();
//            var geraete = BueroMongoContext.Geraete.Find(x => true).ToList();

//            // filter
//            var personenFilter = BueroMongoContext.Personen.Find(x => x.Gebdat < DateTime.Now.AddDays(-5000)).ToList();
//            var geraeteFilter = BueroMongoContext.Geraete.Find(x => x.Person.Equals(personen[0])).ToList();

//            // filter and projektion
//            var personenFilterProjektion = BueroMongoContext.Personen
//                .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
//                .Project(x => x.Name)
//                .ToList();
//            var geraeteFilterProjektion = BueroMongoContext.Geraete
//                .Find(x => x.Person.Equals(personen[0]))
//                .Project(x => x.Name)
//                .ToList();

//            //with filter, projektion, sorting
//            var personenFilterProjektionSorting = BueroMongoContext.Personen
//                .Find(x => x.Gebdat < DateTime.Now.AddDays(-5000))
//                .Project(x => x.Name)
//                .SortBy(x => x.Name)
//                .ToList();
//            var geraeteFilterProjektionSorting = BueroMongoContext.Geraete
//                .Find(x => x.Person.Equals(personen[0]))
//                .Project(x => x.Name)
//                .SortBy(x => x.Name)
//                .ToList();

//            //no filter, aggregation
//            var personenAggregation = BueroMongoContext.Personen.Aggregate()
//                                        .Group(x => x.Gebdat, g =>
//                                            new {
//                                                Max = g.Max(a => DateTime.Now - a.Gebdat)
//                                            }).ToList()[0];
//            var geraeteAggregation = BueroMongoContext.Geraete.Aggregate()
//                                        .Group(x => x.Person, g =>
//                                            new {
//                                                Max = g.Max(a => DateTime.Now - a.Person.Gebdat)
//                                            }).ToList()[0];

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Mongo Update
//        long UpdateMongoTimer()
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            var updatePerson = Builders<MongoPerson>.Update
//            .Set(person => person.Name, "Test");

//            var personen = BueroMongoContext.Personen.UpdateMany(Builders<MongoPerson>.Filter.Where(x => true), updatePerson);

//            var updateGeraet = Builders<MongoGeraet>.Update
//            .Set(geraet => geraet.Name, "Test");

//            var geraete = BueroMongoContext.Geraete.UpdateMany(Builders<MongoGeraet>.Filter.Where(x => true), updateGeraet);

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }

//        //Mongo Delete
//        long DeleteMongoTimer()
//        {
//            Stopwatch timer = new();
//            timer.Start();

//            BueroMongoContext.Personen.DeleteMany(Builders<MongoPerson>.Filter.Where(x => true));
//            BueroMongoContext.Geraete.DeleteMany(Builders<MongoGeraet>.Filter.Where(x => true));

//            timer.Stop();
//            return timer.ElapsedMilliseconds;
//        }
//    }
//}
