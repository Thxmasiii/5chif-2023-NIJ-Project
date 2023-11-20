using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record PersonDTO(int Id, string name, DateTime gebdat, Geschlecht geschlecht, List<LocalRollenDTO> rollen, List<LocalParkplatzDTO> parkplaetze);
    public record LocalPersonDTO(int Id, string name, DateTime gebdat, Geschlecht geschlecht);
}