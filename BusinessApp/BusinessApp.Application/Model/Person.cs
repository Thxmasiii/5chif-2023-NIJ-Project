using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApp.Application.Model
{
    public enum Geschlecht
    {
        Weiblich, Maennlich
    }
    public class Person
    {
        [Column("p_id")]
        public int Id { get; set; }

        [Column("p_name")]
        public string Name { get; set; }

        [Column("p_gebdat")]
        public DateTime Gebdat { get; set; }

        [Column("p_geschlecht")]
        public Geschlecht Geschlecht { get; set; }

        public List<Geraet> Geraete { get; set; } = new();

        public Person(string name, DateTime gebdat, Geschlecht geschlecht)
        {
            Name = name;
            Gebdat = gebdat;
            Geschlecht = geschlecht;
        }
    }
}
