using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Editora> Editoras { get; set; }
        
    }
}
