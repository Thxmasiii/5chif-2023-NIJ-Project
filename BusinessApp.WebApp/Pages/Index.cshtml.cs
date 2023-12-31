﻿using Bogus;
using BusinessApp.Application.Model;
using BusinessApp.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
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

        //varibales for add
        public Guid selectedPerson { get; set; } = new Guid();
        public MongoPerson selectedMongoPerson { get; set; } = new MongoPerson("",DateTime.Now, Application.Infrastructure.BueroMongoContext.Geschlecht.Maennlich);

        [BindProperty]
        public string newArt { get; set; } = "";
        [BindProperty]
        public string newName { get; set; } = "";


        public List<Application.Model.Person> persons { get; set; } = new();
        public List<Geraet> gereate { get; set; } = new();
        public List<MongoPerson> mongoPersons { get; set; } = new();
        public List<MongoGeraet> mongoGereate { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IService _service)
        {
            _logger = logger;
            service = _service;
            (sqlTimer, persons) = service.ReadPersonsNoFilter();
            (mongoTimer, mongoPersons) = service.ReadMongoPersonsNoFilter();
            //MongoPersons = service.BueroMongoContext.Personen.Find(_ => true).ToList();
        }

        //public IndexModel()
        //{

        //}


        public void OnGet()
        {

        }

        public void OnGetGeraete(Guid id, int filter)
        {
            OnGet();
            OnGetChangeFilter(filter);
            selectedPerson = id;
            gereate = service.GetGeraetePerPerson(id);
            Console.WriteLine(gereate.Count);
        }

        public void OnGetMongoGeraete(string id, int filter)
        {
            OnGet();
            OnGetChangeFilter(filter);
            selectedMongoPerson = mongoPersons.Find(x => x.Id == new ObjectId(id));
            mongoGereate = service.GetGeraetePerMongoPerson(id);
            Console.WriteLine(gereate.Count);
        }

        public async Task<IActionResult> OnPostSetGeraete(Guid personid, int filter)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            return RedirectToPage("Index", "Geraete", new { id = personid, filter });
        }

        public async Task<IActionResult> OnPostAddGeraet(Guid personid, int filter)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            service.AddGeraetPostgresTimer(new Geraet(newArt, newName, personid));
            return RedirectToPage("Index", "Geraete", new { id = personid, filter });
        }

        public async Task<IActionResult> OnPostAddMongoGeraet(string personid, int filter)
        {
            MongoPerson person = mongoPersons.Find(x => x.Id == new ObjectId(personid));
            service.AddGeraetMongoTimer(new MongoGeraet(newName, person));
            return RedirectToPage("Index", "MongoGeraete", new { id = personid, filter });
        }

        public async Task<IActionResult> OnPostSetMongoGeraete(string personid, int filter)
        {
            //gereate = service.GetGeraetePerPerson(personid);
            Console.WriteLine("Monog id " + personid);
            return RedirectToPage("Index", "MongoGeraete", new { id = personid, filter });
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
            if(filter == 0){
                (sqlTimer, persons) = service.ReadPersonsNoFilter();
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsNoFilter();
            }                
            else if (filter == 1){
                (sqlTimer, persons) = service.ReadPersonsWithFilter();
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsWithFilter();
            }
            else if(filter == 2)
            {
                (sqlTimer, persons) = service.ReadPersonsWithFilterAndProjektion();
                (mongoTimer, mongoPersons) = service.ReadMongoPersonsWithFilterAndProjection();
            }
            else if(filter == 3){

                (sqlTimer, persons) = service.ReadPersonsWithFilterProjektionAndSorting();
                (mongoTimer, mongoPersons) = service.ReadMongoTimerWithFilterProjektionAndSorting();
            }
           
        }

        public void changeDatabaseStruct(int filter)
        {

        }
    }
}
