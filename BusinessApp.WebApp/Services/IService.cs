using System.Diagnostics;
using BusinessApp.Application.Model;
using MongoDB.Bson;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;

namespace BusinessApp.WebApp.Services
{
    public interface IService
    {
        long CreateAndInsertPostgresTimer(int anz);
        long ReadAllPersons();
        (long, List<Person>) ReadPersonsNoFilter();
        (long, List<Person>) ReadPersonsWithFilter();
        (long, List<Person>) ReadPersonsWithFilterAndProjektion();
        (long, List<Person>) ReadPersonsWithFilterProjektionAndSorting();
        (long, DateTime) ReadPersonsWithAggregation();
        long UpdatePostgresTimer();
        long DeletePostgresTimer();
        long AddGeraetPostgresTimer(Geraet geraet);
        long CreateAndInsertMongoTimer(bool withIndex, int anz);
        List<MongoGeraet> GetGeraetePerMongoPerson(string id);
        long ReadMongoAllMethodes();
        (long, List<MongoPerson>) ReadMongoPersonsNoFilter();
        (long, List<MongoPerson>) ReadMongoPersonsWithFilter();
        (long, List<MongoPerson>) ReadMongoPersonsWithFilterAndProjection();
        (long, List<MongoPerson>) ReadMongoTimerWithFilterProjektionAndSorting();
        (long, DateTime) ReadMongoPersonsWithAggregation();
        long UpdateMongoTimer();
        long DeleteMongoTimer();
        long AddGeraetMongoTimer(MongoGeraet geraet);
        List<Geraet> GetGeraetePerPerson(Guid id);
    }
}
