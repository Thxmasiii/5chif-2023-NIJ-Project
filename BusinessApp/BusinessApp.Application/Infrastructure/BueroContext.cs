using Bogus;
using BusinessApp.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApp.Application.Infrastructure
{
    public class BueroContext : DbContext
    {
        public DbSet<Model.Person> Personen => Set<Model.Person>();
        public DbSet<Geraet> Geraete => Set<Geraet>();

        public string DbPath { get; }

        public BueroContext()
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //DbPath = Path.Join(path, "blogging.db");
            DbPath = "buero.db";
        }

        public BueroContext(DbContextOptions<BueroContext> options) : base(options)
        {

        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        /* protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
                .UseSqlite($"Data Source={DbPath}")
                .LogTo(Console.WriteLine);
                */

        public int SeedBogus(int anz)
        {
            var fakePerson = new Faker<Model.Person>().CustomInstantiator(f =>
            {
                return new Model.Person(f.Person.FullName, f.Date.Recent(10000).ToUniversalTime(), f.PickRandom<Geschlecht>());
            }).Generate(anz).ToList();
            AddRange(fakePerson);
            SaveChanges();

            var fakeGeraet = new Faker<Geraet>().CustomInstantiator(f =>
            {
                return new Geraet(f.Lorem.Word(), f.Lorem.Word(), fakePerson[f.Random.Number(0,anz-1)].Id);
            }).Generate(anz*2).ToList();
            AddRange(fakeGeraet);
            SaveChanges();

            return 0;
        }
    }
}
