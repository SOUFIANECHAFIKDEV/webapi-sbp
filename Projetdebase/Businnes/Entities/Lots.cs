﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("lots")]
    public class Lots
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        public virtual ICollection<LotProduits> LotProduits { get; set; }
    }
}
