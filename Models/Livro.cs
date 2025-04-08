using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Livro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }

        [Required]
        [StringLength(30)]
        public string Nome { get; set; }

        [StringLength(50)]
        public string Descricao { get; set; }

        [Required]
        public int Edicao { get; set; }

        public DateTime Lancamento { get; set; }

        public int? EditoraId { get; set; }

        [ForeignKey("EditoraId")]
        public Editora Editora { get; set; }
    }
}
