using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSDDemo.Data;
using SSDDemo.Models;
using System.Linq;

namespace SSDDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SSDController : ControllerBase
    {
        private readonly DemoTokenContext _context;

        public SSDController(DemoTokenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SSD>> GetList([FromQuery]SSDFilter filter)
        {
            var list = _context.SSD.Where(x => true);

            if (!string.IsNullOrWhiteSpace(filter.Descricao))
                list = list.Where(x => x.Descricao.Contains(filter.Descricao));
            
            if (!string.IsNullOrWhiteSpace(filter.Sigla))
                list = list.Where(x => x.Sigla.Contains(filter.Sigla));
            
            if (!string.IsNullOrWhiteSpace(filter.EmailAtendimento))
                list = list.Where(x => x.EmailAtendimento.Contains(filter.EmailAtendimento));

            return await list.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SSD>> Get(int id)
        {
            return await _context.SSD.FindAsync(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SSD ssd)
        {
            if (id != ssd.Id)
                return BadRequest();
            
            _context.Entry(ssd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SSDExist(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post(SSD ssd)
        {
            _context.SSD.Add(ssd);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("Get", new { id = ssd.Id }, ssd);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ssd = await _context.SSD.FindAsync(id);

            if (ssd == null)
                return NotFound();

            _context.SSD.Remove(ssd);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool SSDExist(int id)
        {
            return _context.SSD.Find(id) != null;
        }
    }
}