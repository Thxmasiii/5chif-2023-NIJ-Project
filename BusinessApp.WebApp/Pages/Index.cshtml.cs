using BusinessApp.Application.Model;
using BusinessApp.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
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
        public List<MongoPerson> mongoPersons { get; set; } = new();
        public List<MongoGeraet> mongoGereate { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IService _service)
        {
            _logger = logger;
            service = _service;
            (sqlTimer, persons) = service.ReadPersonsNoFilter(1);
            (mongoTimer, mongoPersons) = service.ReadMongoPersonsNoFilter(1);
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

        public void OnGetMongoGeraete(ObjectId id)
        {
            OnGet();
            mongoGereate = service.GetGeraetePerMongoPerson(id);
            Console.WriteLine(gereate.Count);
        }

        public async Task<IActionResult> OnPostSetGeraete(int personid)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            return RedirectToPage("Index", "Geraete", new { id = personid });
        }

        public async Task<IActionResult> OnPostSetMongoGeraete(ObjectId personid)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            return RedirectToPage("Index", "MongoGeraete", new { id = new ObjectId(personid.ToString()) });
        }

        public async Task<IActionResult> OnPostFilter(int f)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            Console.WriteLine("Post");
            return RedirectToPage("Index", "ChangeFilter", new { filter = f });
        }

        public void OnGetChangeFilter(int filter)
        {
            if(filter == 0){
                (sqlTimer, persons) = service.ReadPersonsNoFilter(1);
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsNoFilter(1);
            }                
            else if (filter == 1){
                (sqlTimer, persons) = service.ReadPersonsWithFilter(1);
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsWithFilter(1);
            }
            else if(filter == 2)
            {
                (sqlTimer, persons) = service.ReadPersonsWithFilterAndProjektion(1);
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsWithFilterAndProjection(1);
            }
            else if(filter == 3){

                (sqlTimer, persons) = service.ReadPersonsWithFilterProjektionAndSorting(1);
                (mongoTimer, mongoPersons) = service.ReadMongoTimerWithFilterProjektionAndSorting(1);
            }
           
        }

        public void changeDatabaseStruct(int filter)
        {

        }
    }
}
