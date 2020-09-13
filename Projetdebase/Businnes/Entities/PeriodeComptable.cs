using InoAuthentification.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Table("periode_comptable")]
    public class PeriodeComptable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }


        [Column("date_debut")]
        public DateTime DateDebut { get; set; }

        [Column("periode")]
        public int Periode { get; set; }

        [Column("date_cloture")]
        public DateTime? DateCloture { get; set; }

        [Column("id_user")]
        [ForeignKey("User")]
        public int? IdUser { get; set; }


        public User User { get; set; }
    }
}
