using BusinessApp.Application.Model;

namespace BusinessApp.WebApp.Services
{
    public interface IService
    {
        long CreateAndInsertPostgresTimer(int anz);
        long ReadPostgresTimer(int anz, int filter);
        long UpdatePostgresTimer(int anz);
        long DeletePostgresTimer(int anz);
        long CreateAndInsertMongoTimer(bool withIndex, int anz);
        long ReadMongoTimer(bool withIndex, int anz, int filter);
        long UpdateMongoTimer(bool withIndex, int anz);
        long DeleteMongoTimer(bool withIndex, int anz);
        List<Geraet> GetGeraetePerPerson(int id);
    }
}
