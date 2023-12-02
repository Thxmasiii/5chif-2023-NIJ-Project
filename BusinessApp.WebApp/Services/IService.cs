using System.Diagnostics;
using BusinessApp.Application.Model;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;

namespace BusinessApp.WebApp.Services
{
    public interface IService
    {
        long CreateAndInsertPostgresTimer(int anz);
        (long, List<Person>) ReadPersonsNoFilter(int anz);
        (long, List<Person>) ReadPersonsWithFilter(int anz);
        (long, List<Person>) ReadPersonsWithFilterAndProjektion(int anz);
        (long, List<Person>) ReadPersonsWithFilterProjektionAndSorting(int anz);
        (long, DateTime) ReadPersonsWithAggregation(int anz);
        long UpdatePostgresTimer(int anz);
        long DeletePostgresTimer(int anz);
        long AddGeraetPostgresTimer(Geraet geraet);
        long CreateAndInsertMongoTimer(bool withIndex, int anz);
        (long, List<MongoPerson>) ReadMongoPersonsNoFilter(int anz);
        (long, List<MongoPerson>) ReadMongoPersonsWithFilter(int anz);
        (long, List<MongoPerson>) ReadMongoPersonsWithFilterAndProjection(int anz);
        (long, List<MongoPerson>) ReadMongoTimerWithFilterProjektionAndSorting(int anz);
        (long, DateTime) ReadMongoPersonsWithAggregation(int anz);
        long UpdateMongoTimer(bool withIndex, int anz);
        long DeleteMongoTimer(bool withIndex, int anz);
        long AddGeraetMongoTimer(MongoGeraet geraet);
        List<Geraet> GetGeraetePerPerson(int id);
    }
}
