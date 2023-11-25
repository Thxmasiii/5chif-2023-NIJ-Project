using BusinessApp.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApp.Test.Database
{
    public class BueroContextTests : IClassFixture<DatabaseFixtureBogus>
    {
        DatabaseFixtureBogus _fixture;
        BueroContext    _dbContext;
        public BueroContextTests(DatabaseFixtureBogus fixture)
        {
            _fixture = fixture;
            _dbContext = fixture.Context;
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await _dbContext.Database.CanConnectAsync());
        }

        [Fact]
        public void SeedSuccessTest()
        {
            Assert.True(_dbContext.Geraete.ToList().Count > 0);
            Assert.True(_dbContext.Personen.ToList().Count > 0);
        }
    }
}
