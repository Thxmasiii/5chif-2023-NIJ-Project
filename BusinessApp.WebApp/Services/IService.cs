using BusinessApp.Application.Model;

namespace BusinessApp.WebApp.Services
{
    public interface IService
    {
        long CreateAndInsertPostgresTimer(int anz);
        long UpdatePostgresTimer(int anz);
        long DeletePostgresTimer(int anz);
        long CreateAndInsertMongoTimer(bool withIndex, int anz);
        long ReadMongoTimer(bool withIndex, int anz, int filter);
        long UpdateMongoTimer(bool withIndex, int anz);
        long DeleteMongoTimer(bool withIndex, int anz);
        (long, List<Person>) ReadPersonsNoFilter(int anz);
        (long, List<Person>) ReadPersonsWithFilter(int anz);
        (long, List<Person>) ReadPersonsWithFilterAndProjektion(int anz);
        (long, List<Person>) ReadPersonsWithFilterProjektionAndSorting(int anz);
        (long, DateTime) ReadPersonsWithAggregation(int anz);

        List<Geraet> GetGeraetePerPerson(int id);
    }
}
