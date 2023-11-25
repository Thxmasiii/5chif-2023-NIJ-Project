using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApp.Application.Model
{
    public class Geraet
    {
        [Column("g_id")]
        public int Id { get; set; }

        [Column("g_art")]
        public string Art { get; set; }

        [Column("g_name")]
        public string Name { get; set; }

        [Column("g_person")]
        public int PersonId { get; set; }

        public Person Person { get; set; }


        public Geraet(string art, string name, int personId)
        {
            Art = art;
            Name = name;
            PersonId = personId;
        }
    }
}
