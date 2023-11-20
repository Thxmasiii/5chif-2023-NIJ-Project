using BusinessApp.Application.Model;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BusinessApp.WebAPI.DTO.Converter
{
    public class DTOConverter
    {
        public LocalBueroDTO BueroToLocalBueroDTO(Buero b)
        {
            return new LocalBueroDTO(b.Id, b.Adresse, b.PLZ);
        }

        public LocalParkplatzDTO ParkplatzToLocalParkplatzDTO(Parkplatz p)
        {
            return new LocalParkplatzDTO(p.Id, p.Nr, p.Indoor);
        }

        public LocalRaumDTO RaumToLocalRaumDTO(Raum r)
        {
            return new LocalRaumDTO(r.Id, r.Nr, r.Sitze);
        }
        public LocalRollenDTO RollenToLocalRollenDTO(Rolle r)
        {
            return new LocalRollenDTO(r.Id,r.Name);
        }

        public List<LocalParkplatzDTO> ToLocalParkplatzDTOList(List<Parkplatz> pl)
        {
            var result = new List<LocalParkplatzDTO>();
            pl.ForEach(x => result.Add(ParkplatzToLocalParkplatzDTO(x)));
            return result;
        }

        public List<LocalRaumDTO> ToLocalRaumDTOList(List<Raum> rl)
        {
            var result = new List<LocalRaumDTO>();
            rl.ForEach(x => result.Add(RaumToLocalRaumDTO(x)));
            return result;
        }

        public List<LocalRollenDTO> ToLocalRollenDTOList(List<Rolle> rl)
        {
            var result = new List<LocalRollenDTO>();
            rl.ForEach(x => result.Add(RollenToLocalRollenDTO(x)));
            return result;
        }

        public List<Raum> DTOtoRaumList(List<LocalRaumDTO> rl, Buero b, List<Raum> dbRaeume)
        {
            List<Raum> results = new List<Raum>();
            rl.ForEach(r => {
                List<Raum> raumlist = dbRaeume.Where(x => x.Id == r.Id).ToList();
                if(raumlist.Count == 0)
                {
                    results.Add(new Raum(r.nr, r.sitze, b.Id));
                }
                else
                {
                    results.Add(raumlist[0]);
                }
            });
            Console.WriteLine("resultscount: " + results.Count);
            return results; 
        }

        public List<Parkplatz> DTOtoParkplatzList(List<LocalParkplatzDTO> pl, Buero b, List<Parkplatz> dbParkplatz, Person? p)
        {
            List<Parkplatz> results = new List<Parkplatz>();
            pl.ForEach(p => {
                List<Parkplatz> parkplatzlist = dbParkplatz.Where(x => x.Id == p.Id).ToList();
                if (parkplatzlist.Count == 0)
                {
                    results.Add(new Parkplatz(p.nr,p.indoor,b.Id, p.Id));
                }
                else
                {
                    results.Add(parkplatzlist[0]);
                }
            });
            Console.WriteLine("resultscount: " + results.Count);
            return results;
        }

        public List<Rolle> DTOtoRollenList(List<LocalRollenDTO> rl, Person p, List<Rolle> dbRolles)
        {
            List<Rolle> results = new List<Rolle>();
            rl.ForEach(r => {
                List<Rolle> rollenlist = dbRolles.Where(x => x.Id == p.Id).ToList();
                if (rollenlist.Count == 0)
                {
                    results.Add(new Rolle(p.Name));
                }
                else
                {
                    results.Add(rollenlist[0]);
                }
            });
            Console.WriteLine("resultscount: " + results.Count);
            return results;
        }
    }
}
