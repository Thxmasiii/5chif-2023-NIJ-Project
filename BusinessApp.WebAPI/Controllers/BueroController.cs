using BusinessApp.Application.Infrastructure;
using BusinessApp.Application.Model;
using BusinessApp.WebAPI.DTO;
using BusinessApp.WebAPI.DTO.Converter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.WebAPI.Controllers
{
    /// <summary>
    /// Reagiert auf /api/Buero
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BueroController : ControllerBase
    {
        // Der Controller wird bei jedem Request neu instanzierst. Damit unsere Änderungen erhalten
        // bleiben verwenden wir static.
        private readonly ILogger<BueroController> _logger;
        private readonly BueroContext db;
        private DTOConverter conv = new DTOConverter();

        public BueroController(ILogger<BueroController> logger, BueroContext _db)
        {
            _logger = logger;
            db = _db;
        }

        /// <summary>
        /// Reagiert auf /api/Buero
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Buero> GetBueros()
        {
            var result = new List<BueroDTO>();
            db.Bueros.Include(x => x.Raeume).Include(x => x.Parkplaetze).ToList().ForEach(b => result.Add(new BueroDTO(b.Id,b.Adresse,b.PLZ, conv.ToLocalRaumDTOList(b.Raeume), conv.ToLocalParkplatzDTOList(b.Parkplaetze))));
            return Ok(result);
        }

        /// <summary>
        /// Reagiert auf /api/Buero/{id}
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Buero> GetBueroById(int id)
        {
            BueroDTO b;
            var result = new List<BueroDTO>();
            db.Bueros.Include(x => x.Raeume).Include(x => x.Parkplaetze).ToList().ForEach(b => result.Add(new BueroDTO(b.Id, b.Adresse, b.PLZ, conv.ToLocalRaumDTOList(b.Raeume), conv.ToLocalParkplatzDTOList(b.Parkplaetze))));
            b = result.SingleOrDefault(b => b.Id == id);
            if (b == null) { return NotFound(); }
            return Ok(b);
        }


        /// <summary>
        /// Reagiert auf POST /api/Buero
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Buero> Post(BueroDTO buero)
        {
            // Falls Fehler passieren, kann dies in diesem Code nur Serverseitige Ursachen haben. Daher
            // wird kein try und catch verwendet.
            // Wenn PK Konflikte auftreten, würden wir Conflict() senden. Verletzt der Datensatz
            // Constraints, senden wir BadRequest().
            int newId = db.Bueros.Max(p => p.Id) + 1;          // Simuliert eine Autoincrement Id.
            Buero b = new Buero(buero.adresse, buero.pLZ);
            b.Id = newId;
            b.Raeume.AddRange(conv.DTOtoRaumList(buero.raeume,b,db.Raeume.ToList()));
            db.Bueros.Add(b);
            db.SaveChangesAsync();

            // Liefert den Inhalt des Requests GET /api/Buero/{id} und sendet 201Created.
            return CreatedAtAction(nameof(GetBueroById), new { id = b.Id }, b);
        }

        /// <summary>
        /// Reagiert auf POST /api/Buero/fromForm
        /// </summary>
        [HttpPost("fromForm")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Buero> PostFromForm([FromForm] BueroDTO Buero) => Post(Buero);


        /// <summary>
        /// Reagiert auf PUT /api/Buero/{id}
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Buero> Put(int id, BueroDTO buero)
        {
            try
            {
                // Wenn der PK des Buero Objektes vom Parameter der Anfrage abweicht, 
                // senden wir HTTP 409 (Conflict).
                if (id != buero.Id) { return Conflict(); }
                Buero found = db.Bueros.Include(x => x.Raeume).Include(x => x.Parkplaetze).SingleOrDefault(p => p.Id == id);
                if (found == null) { return NotFound(); }
                // Simuliert das Aktualisieren des Datensatzes in der Db. Dabei darf der Primärschlüssel
                // (die Id) nicht geändert werden. Ob alle anderen Properties geändert werden dürfen
                // ist natürlich Sache der Programmlogik.
                found.Adresse = buero.adresse;
                found.Raeume = conv.DTOtoRaumList(buero.raeume, found, db.Raeume.ToList());
                found.Parkplaetze = conv.DTOtoParkplatzList(buero.parkplaetze,found,db.Parkplaetze.ToList(), null);
                found.PLZ = buero.pLZ;
                // Der PUT Request sendet keinen Inhalt, nur HTTP 204
                return NoContent();
            }
            catch
            {
                // Bei einer PK Kollision würden wir Conflict() senden. Da wir Autowerte haben,
                // kommt das aber nicht vor.
                // Wenn hier ein Fehler auftritt, muss das also serverseitige Ursachen haben. Daher
                // geben wir den Fehler weiter, der dann als HTTP 500 gesendet wird.
                throw;
            }
        }

        /// <summary>
        /// Reagiert auf DELETE /api/Buero/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public StatusCodeResult Delete(int id)
        {
            try
            {
                Buero found = db.Bueros.FirstOrDefault(p => p.Id == id);
                if (found == null) { return NotFound(); }
                db.Bueros.Remove(found);
                db.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                // Beziehung stehen, senden wir Conflict().
                // Wenn hier ein Fehler auftritt, muss das also serverseitige Ursachen haben. Daher
                // geben wir den Fehler weiter, der dann als HTTP 500 gesendet wird.
                throw;
            }
        }
    }
}