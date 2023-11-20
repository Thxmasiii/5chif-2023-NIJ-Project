using BusinessApp.Application.Model;

namespace BusinessApp.WebAPI.DTO
{
    public record RaumDTO(int Id, string nr, int sitze, LocalBueroDTO? buero, List<LocalGeraetDTO> geraete);
    public record LocalRaumDTO(int Id, string nr, int sitze);
}
