//using BusinessApp.Application.Infrastructure;
//using BusinessApp.Application.Model;
//using BusinessApp.WebAPI.DTO;
//using BusinessApp.WebAPI.DTO.Converter;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BusinessApp.WebAPI.Controllers
//{
//    /// <summary>
//    /// Reagiert auf /api/Person
//    /// </summary>
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PersonController : ControllerBase
//    {
//        // Der Controller wird bei jedem Request neu instanzierst. Damit unsere Änderungen erhalten
//        // bleiben verwenden wir static.
//        private readonly ILogger<PersonController> _logger;
//        private readonly BueroContext db;
//        //private DTOConverter conv = new DTOConverter();

//        public PersonController(ILogger<PersonController> logger, BueroContext _db)
//        {
//            _logger = logger;
//            db = _db;
//        }

//        /// <summary>
//        /// Reagiert auf /api/Person
//        /// </summary>
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public ActionResult<Person> GetPerson()
//        {
//            var result = new List<PersonDTO>();
//            db.Personen.Include(x => x.Rollen).Include(x => x.Parkplaetze).ToList().ForEach(p => result.Add(new PersonDTO(p.Id,p.Name,p.Gebdat,p.Geschlecht,conv.ToLocalRollenDTOList(p.Rollen), conv.ToLocalParkplatzDTOList(p.Parkplaetze))));
//            return Ok(result);
//        }

//        /// <summary>
//        /// Reagiert auf /api/Person/{id}
//        /// </summary>
//        [HttpGet("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public ActionResult<Person> GetPersonById(int id)
//        {
//            PersonDTO b;
//            var result = new List<PersonDTO>();
//            db.Personen.Include(x => x.Rollen).Include(x => x.Parkplaetze).ToList().ForEach(p => result.Add(new PersonDTO(p.Id, p.Name, p.Gebdat, p.Geschlecht, conv.ToLocalRollenDTOList(p.Rollen), conv.ToLocalParkplatzDTOList(p.Parkplaetze))));
//            b = result.SingleOrDefault(b => b.Id == id);
//            if (b == null) { return NotFound(); }
//            return Ok(b);
//        }


//        /// <summary>
//        /// Reagiert auf POST /api/Person
//        /// </summary>
//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        public ActionResult<Person> Post(PersonDTO person)
//        {
//            // Falls Fehler passieren, kann dies in diesem Code nur Serverseitige Ursachen haben. Daher
//            // wird kein try und catch verwendet.
//            // Wenn PK Konflikte auftreten, würden wir Conflict() senden. Verletzt der Datensatz
//            // Constraints, senden wir BadRequest().
//            int newId = db.Personen.Max(p => p.Id) + 1;          // Simuliert eine Autoincrement Id.
//            Person p = new Person(person.name, person.gebdat.ToUniversalTime(), person.geschlecht);
//            p.Id = newId;
//            //b.Rollen.AddRange(conv.DTOtoRollenList(person.rollen,b,db.Rollen.ToList()));
//            db.Personen.Add(p);
//            db.SaveChanges();

//            // Liefert den Inhalt des Requests GET /api/Person/{id} und sendet 201Created.
//            return CreatedAtAction(nameof(GetPersonById), new { id = p.Id }, p);
//        }

//        /// <summary>
//        /// Reagiert auf POST /api/Person/fromForm
//        /// </summary>
//        [HttpPost("fromForm")]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        public ActionResult<Person> PostFromForm([FromForm] PersonDTO Person) => Post(Person);


//        /// <summary>
//        /// Reagiert auf PUT /api/Person/{id}
//        /// </summary>
//        [HttpPut("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public ActionResult<Person> Put(int id, PersonDTO person)
//        {
//            try
//            {
//                // Wenn der PK des Person Objektes vom Parameter der Anfrage abweicht, 
//                // senden wir HTTP 409 (Conflict).
//                if (id != person.Id) { return Conflict(); }
//                Person found = db.Personen.Include(x => x.Rollen).Include(x => x.Parkplaetze).SingleOrDefault(p => p.Id == id);
//                if (found == null) { return NotFound(); }
//                // Simuliert das Aktualisieren des Datensatzes in der Db. Dabei darf der Primärschlüssel
//                // (die Id) nicht geändert werden. Ob alle anderen Properties geändert werden dürfen
//                // ist natürlich Sache der Programmlogik.
//                found.Geschlecht = person.geschlecht;
//                found.Rollen = conv.DTOtoRollenList(person.rollen, found, db.Rollen.ToList());
//                found.Parkplaetze = conv.DTOtoParkplatzList(person.parkplaetze, db.Bueros.ToList()[0], db.Parkplaetze.ToList(), found);
//                found.Gebdat = person.gebdat;
//                found.Name = person.name;
//                // Der PUT Request sendet keinen Inhalt, nur HTTP 204
//                return NoContent();
//            }
//            catch
//            {
//                // Bei einer PK Kollision würden wir Conflict() senden. Da wir Autowerte haben,
//                // kommt das aber nicht vor.
//                // Wenn hier ein Fehler auftritt, muss das also serverseitige Ursachen haben. Daher
//                // geben wir den Fehler weiter, der dann als HTTP 500 gesendet wird.
//                throw;
//            }
//        }

//        /// <summary>
//        /// Reagiert auf DELETE /api/Person/{id}
//        /// </summary>
//        [HttpDelete("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public StatusCodeResult Delete(int id)
//        {
//            try
//            {
//                Person found = db.Personen.FirstOrDefault(p => p.Id == id);
//                if (found == null) { return NotFound(); }
//                db.Personen.Remove(found);
//                db.SaveChangesAsync();
//                return NoContent();
//            }
//            catch
//            {
//                // Beziehung stehen, senden wir Conflict().
//                // Wenn hier ein Fehler auftritt, muss das also serverseitige Ursachen haben. Daher
//                // geben wir den Fehler weiter, der dann als HTTP 500 gesendet wird.
//                throw;
//            }
//        }
//    }
//}