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

        [BindProperty]
        public int Filter { get; set; } = 0;

        long sqlTimer = 0;
        long mongoTimer = 0;

        public List<Person> persons { get; set; } = new();
        public List<Geraet> gereate { get; set; } = new();
        public List<MongoPerson> MongoPersons { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IService _service)
        {
            _logger = logger;
            service = _service;
            (sqlTimer, persons) = service.ReadPersonsNoFilter();
            //MongoPersons = service.BueroMongoContext.Personen.Find(_ => true).ToList();
        }

        //public IndexModel()
        //{

        //}


        public void OnGet()
        {

        }

        public void OnGetGeraete(Guid id)
        {
            OnGet();
            gereate = service.GetGeraetePerPerson(id);
            Console.WriteLine(gereate.Count);
        }

        public async Task<IActionResult> OnPostSetGeraete(Guid personid)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            return RedirectToPage("Index", "Geraete", new { id = personid });
        }

        public async Task<IActionResult> OnPostFilter(int f)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            Console.WriteLine("Post");
            return RedirectToPage("Index", "ChangeFilter", new { filter = f });
        }

        public void OnGetChangeFilter(int filter)
        {
            Filter = filter;
            if(filter == 0)
           (sqlTimer, persons) = service.ReadPersonsNoFilter();
            else if (filter == 1)
                (sqlTimer, persons) = service.ReadPersonsWithFilter();
            else if(filter == 2)
                (sqlTimer, persons) = service.ReadPersonsWithFilterAndProjektion();
            else if(filter == 3)
                (sqlTimer, persons) = service.ReadPersonsWithFilterProjektionAndSorting();
        }

        public void changeDatabaseStruct(int filter)
        {

        }
    }
}
