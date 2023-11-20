using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record ParkplatzDTO(int Id, int nr, bool indoor, LocalBueroDTO? buero, LocalPersonDTO? person);
    public record LocalParkplatzDTO(int Id, int nr, bool indoor);
}
