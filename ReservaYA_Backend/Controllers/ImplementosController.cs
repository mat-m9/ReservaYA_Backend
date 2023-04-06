using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;

namespace ReservaYA_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImplementosController : ControllerBase
    {
        private readonly DatabaseContext context;

        public ImplementosController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ImplementoModel>>> Get()
        {
            var Implemento = await context.Implementos.Where(e => e.Cant > 0).ToListAsync();
            if (!Implemento.Any())
                return NotFound();
            return Implemento;
        }
    }
}
