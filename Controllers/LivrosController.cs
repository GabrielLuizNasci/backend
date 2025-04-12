using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly Contexto _context;

        public LivrosController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Livros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livro>>> GetLivros()
        {
            var livros = await _context.Livros
            .Include(l => l.Editora)
            .Select(l => new
            {
                l.Id,
                l.Nome,
                l.Descricao,
                l.Edicao,
                l.Lancamento,
                l.EditoraId,
                NomeEditora = l.Editora != null ? l.Editora.Nome : null
            })
            .ToListAsync();

            return Ok(livros);
        }

        // GET: api/Livros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Livro>> GetLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
            {
                return NotFound();
            }

            return livro;
        }

        // PUT: api/Livros/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, Livro livro)
        {
            if (id != livro.Id)
            {
                return BadRequest();
            }

            var livroExistente = await _context.Livros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (livroExistente == null)
            {
                return NotFound();
            }

            _context.Entry(livro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Se a editora mudou, atualizar as duas
                if (livroExistente.EditoraId != livro.EditoraId)
                {
                    // Atualiza a editora antiga
                    if (livroExistente.EditoraId != null)
                    {
                        var editoraAntiga = await _context.Editoras.FindAsync(livroExistente.EditoraId);
                        if (editoraAntiga != null)
                        {
                            editoraAntiga.QuantLivros = await _context.Livros.CountAsync(l => l.EditoraId == editoraAntiga.Id);
                        }
                    }

                    // Atualiza a nova editora
                    if (livro.EditoraId != null)
                    {
                        var editoraNova = await _context.Editoras.FindAsync(livro.EditoraId);
                        if (editoraNova != null)
                        {
                            editoraNova.QuantLivros = await _context.Livros.CountAsync(l => l.EditoraId == editoraNova.Id);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Livros
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Livro>> PostLivro(Livro livro)
        {
            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            if (livro.EditoraId != null)
            {
                var editora = await _context.Editoras.FindAsync(livro.EditoraId);

                if (editora != null)
                {
                    editora.QuantLivros = await _context.Livros.CountAsync(l => l.EditoraId == editora.Id);
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction("GetLivro", new { id = livro.Id }, livro);
        }

        // DELETE: api/Livros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                return NotFound();
            }

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id);
        }
    }
}
