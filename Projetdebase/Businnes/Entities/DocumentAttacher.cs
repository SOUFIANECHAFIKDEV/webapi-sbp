using ProjetBase.Businnes.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("document_attacher")]
    public class DocumentAttacher
    {
        

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("commentaire")]
        public string Commentaire { get; set; }


        [Column("pieceJointes")]
        public string PieceJointes { get; set; }

        [Column("designation")]
        public string Designation { get; set; }

        [Column("label_document")]
        public string LabelDocument { get; set; }


        [Column("date_ajout")]
        public DateTime DateAjout { get; set; }
        //public long DateAjout { get; set; } = EntityExtensions.UnixTimestampFromDateTime(DateTime.Now);


        [Column("date_derniere_modification")]
        //public long UpdateAt { get; set; } = EntityExtensions.UnixTimestampFromDateTime(DateTime.Now);
        public DateTime UpdateAt { get; set; }


        [Column("idUser")]
        public int IdUser { get; set; }

        [Column("idRubrique")]
        [ForeignKey("Rubrique")]
        public int? idRubrique { get; set; }

        [Column("id_chantier")]
        [ForeignKey("Chantier")]
        public int? IdChantier { get; set; }

        public Chantier Chantier { get; set; }

        public Rubrique Rubrique { get; set; }
    }
}
