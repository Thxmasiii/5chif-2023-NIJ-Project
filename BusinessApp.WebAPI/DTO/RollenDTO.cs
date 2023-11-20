using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record RollenDTO(int Id, string name, List<LocalPersonDTO> personen);
    public record LocalRollenDTO(int Id, string name);
}
