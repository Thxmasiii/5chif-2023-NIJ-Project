//using BusinessApp.Application.Infrastructure;
//using BusinessApp.Application.Model;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Diagnostics;
//using System.Linq;
//using Xunit;

//namespace BusinessApp.Test.Database
//{
//    public class PostDbTestsBogus : IClassFixture<DatabaseFixtureBogus>
//    {
//        DatabaseFixtureBogus _fixture;
//        BueroContext    _db;
//        public PostDbTestsBogus(DatabaseFixtureBogus fixture)
//        {
//            this._fixture = fixture;
//            _db = fixture.Context;
//        }


//        [Fact]
//        public async void AddedItemsShouldGetPersisted()
//        {
//            var newItem = new Buero("Testadresse", "1050");
//            newItem.Raeume.Add(new Raum("1",10,1));
//            newItem.Raeume.Add(new Raum("2", 10, 1));

//            // Posts from seeding are also available
//            int expected = _db.Raeume.Count() + 2;
//            _db.Bueros.Add(newItem);
//            await _db.SaveChangesAsync();

//            // New context! IMPORTANT!!!
//            using var db = _fixture.NewContext();
//            Assert.Equal(expected, db.Raeume.Count());

//            var buero = db.Bueros
//                .Include(x => x.Raeume)      // IMPORTANT!
//                .FirstOrDefault(x => x.Id == newItem.Id);
                
//            Assert.NotNull(buero);

//            Assert.Equal(2, buero!.Raeume.Count);
//        }

//        [Fact]
//        public async void DeletedItemsShouldGetPersisted()
//        {
//            // Posts from seeding are also available
//            int expected = _db.Raeume.Count() - 1;
//            _db.Raeume.Remove(_db.Raeume.ToList()[0]);
//            await _db.SaveChangesAsync();

//            // New context! IMPORTANT!!!
//            using var db = _fixture.NewContext();
//            Assert.Equal(expected, db.Raeume.Count());
//        }
//    }
//}