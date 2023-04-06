using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;

namespace ReservaYA_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorariosController : ControllerBase
    {
        private readonly DatabaseContext context;

        public HorariosController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet(template: ApiRoutes.Horario.Coliseo)]
        public async Task<ActionResult<ICollection<HorarioModel>>> GetHorarioColiseo(string dia)
        {
            var Horario = await context.Horarios.Where(i => i.Coliseo == true).Where(e => e.Dia.Equals(dia)).ToListAsync();
            if (Horario == null)
                return NotFound();
            return Ok(Horario);
        }

        [HttpGet(template: ApiRoutes.Horario.Cancha)]
        public async Task<ActionResult<ICollection<HorarioModel>>> GetHorarioCancha(string dia)
        {
            var Horario = await context.Horarios.Where(i => i.Cancha == true).Where(e => e.Dia.Equals(dia)).ToListAsync();
            if (Horario == null)
                return NotFound();
            return Ok(Horario);
        }

    }
}
