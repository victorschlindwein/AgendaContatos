using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuloAPI.Context;
using ModuloAPI.Entities;

namespace ModuloAPI.Controllers
{
    [ApiController]
    [Route("Contato")]
    public class ContatoController : ControllerBase
    {
        private readonly AgendaContext _context;
        public ContatoController(AgendaContext context)
        {
            _context = context;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int contatoId)
        {
            var contato = await _context.Contatos
                .Include(c => c.Enderecos)
                .FirstOrDefaultAsync(c => c.ContatoId == contatoId);

            return contato == null ? NoContent() : Ok(contato);
        }

        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var contato = await _context.Contatos.Where(x => x.Email.Equals(email))
                .Include(c => c.Enderecos)
                .ToListAsync();

            return contato == null ? NoContent() : Ok(contato);
        }

        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            var contato = await _context.Contatos.Where(x => x.Nome.Equals(name))
                .Include (c => c.Enderecos)
                .ToListAsync();

            return contato == null ? NoContent() : Ok(contato);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllContacts()
        {
            List<Contato> contato = await _context.Contatos
                .Include(c => c.Enderecos)
                .ToListAsync();

            return contato == null ? NoContent() : Ok(contato);
        }

        [HttpPost("NewContact")]
        public async Task<IActionResult> Create(Contato contato)
        {
            contato.DataDeCriacao = DateTime.Now;
            await _context.AddAsync(contato);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = contato.ContatoId }, contato);
        }

        [HttpPut("EditContact/{id}")]
        public async Task<IActionResult> Update(int id, Contato contato)
        {
            var contatoBanco = await _context.Contatos.FindAsync(id);
            if (contatoBanco == null)
                return NoContent();

            contatoBanco.Nome = contato.Nome;
            contatoBanco.Telefone = contato.Telefone;
            contatoBanco.Ativo = contato.Ativo;
            contatoBanco.Email = contato.Email;

            _context.Contatos.Update(contatoBanco);
            await _context.SaveChangesAsync();

            return Ok(contatoBanco);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contatoBanco = _context.Contatos.Find(id);
            if (contatoBanco == null)
                return NoContent();

            _context.Contatos.Remove(contatoBanco);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registro removido do banco" });
        }
    }
}