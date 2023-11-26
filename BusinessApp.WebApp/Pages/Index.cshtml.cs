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
        Service service;

        public List<Person> persons { get; set; } = new();
        List<MongoPerson> MongoPersons = new();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IndexModel(Service _service)
        {
            service = _service;
            persons = service.BueroContext.Personen.ToList();
            MongoPersons = service.BueroMongoContext.Personen.Find(_ => true).ToList();
        }

        public void OnGet()
        {

        }

        public void changeFilter(int filter)
        {
            
        }
    }
}