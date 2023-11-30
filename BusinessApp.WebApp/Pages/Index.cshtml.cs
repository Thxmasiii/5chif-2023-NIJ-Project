using BusinessApp.Application.Model;
using BusinessApp.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using static BusinessApp.Application.Infrastructure.BueroMongoContext;

namespace BusinessApp.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        IService service;

        long sqlTimer = 0;
        long mongoTimer = 0;

        public List<Person> persons { get; set; } = new();
        public List<Geraet> gereate { get; set; } = new();
        public List<MongoPerson> MongoPersons { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IService _service)
        {
            _logger = logger;
            service = _service;
            (sqlTimer, persons) = service.ReadPersonsNoFilter(1);
            //MongoPersons = service.BueroMongoContext.Personen.Find(_ => true).ToList();
        }

        //public IndexModel()
        //{

        //}


        public void OnGet()
        {

        }

        public void OnGetGeraete(int id)
        {
            OnGet();
            gereate = service.GetGeraetePerPerson(id);
            Console.WriteLine(gereate.Count);
        }

        public async Task<IActionResult> OnPostSetGeraete(int personid)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            return RedirectToPage("Index", "Geraete", new { id = personid });
        }

        public void changeFilter(int filter)
        {
            
        }

        public void changeDatabaseStruct(int filter)
        {

        }
    }
}