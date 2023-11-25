using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BusinessApp.Application.Infrastructure;

namespace BusinessApp.Test.Database
{
    public class DatabaseFixtureBogus : IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly BueroContext _db;
        public BueroContext Context => _db;

        // share an in-memory SQLite database in the same process
        //const string connectionString = "Data Source=InMemoryBlogs;Mode=Memory;Cache=Shared";
        // Better: new db for every test class
        const string connectionString = "Data Source=InMemoryBlogs;Mode=Memory";

        public BueroContext NewContext()
        {
            return new BueroContext(new DbContextOptionsBuilder<BueroContext>()
                .UseSqlite(_connection)
                .Options);
        }
        public DatabaseFixtureBogus()
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            var opt = new DbContextOptionsBuilder<BueroContext>()
                .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
                .Options;

            _db = new BueroContext(opt);
        }

        public async Task InitializeAsync()
        {
            // ... initialize data in the test database and/or use async calls here ...
            await _db.Database.EnsureCreatedAsync();
            await _db.SeedBogusAsync(50);
        }

        public async Task DisposeAsync()
        {
            // ... clean up test data from the database with and use async calls here...
            await _db.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
