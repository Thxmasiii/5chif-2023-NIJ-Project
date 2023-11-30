using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record PersonDTO(int Id, string name, DateTime gebdat, Geschlecht geschlecht, List<LocalGeraetDTO> geraete);
    public record LocalPersonDTO(int Id, string name, DateTime gebdat, Geschlecht geschlecht);
}