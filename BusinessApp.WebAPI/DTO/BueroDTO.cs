using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record BueroDTO(int Id, string adresse, string pLZ, List<LocalRaumDTO> raeume, List<LocalParkplatzDTO> parkplaetze);
    public record LocalBueroDTO(int Id, string adresse, string plz);
}

