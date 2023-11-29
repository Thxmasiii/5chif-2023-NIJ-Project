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

        //public List<Person> persons { get; set; } = new();
        //List<MongoPerson> MongoPersons = new();

        public IndexModel(ILogger<IndexModel> logger, IService _service)
        {
            _logger = logger;
            service = _service;
            //persons = service.BueroContext.Personen.ToList();
            //MongoPersons = service.BueroMongoContext.Personen.Find(_ => true).ToList();
        }

        //public IndexModel()
        //{
            
        //}

        public void OnGet()
        {

        }

        public void changeFilter(int filter)
        {
            
        }
    }
}