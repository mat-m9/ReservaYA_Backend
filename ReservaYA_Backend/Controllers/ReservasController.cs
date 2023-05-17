using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;
using ReservaYA_Backend.ResponseModels;

namespace ReservaYA_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly DatabaseContext context;

        public ReservasController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet("id")]
        public async Task<ActionResult<ICollection<ReservasResponse>>> GetReservas(string id)
        {
            List<ReservasResponse> reservasU = new List<ReservasResponse>();
            var reservasImp = await context.ReservaImplementos.Where(u => u.User_ID == id).ToListAsync();
            var reservasIns = await context.ReservaInstalaciones.Where(u => u.User_ID == id).ToListAsync();
            foreach (ReservaImpModel reserva in reservasImp) { 
                ReservasResponse reservasT= new ReservasResponse();
                reservasT.ID = reserva.ID;
                reservasT.Tipo = reserva.Tipo;
                reservasT.Desc = reserva.Dia;
                reservasU.Add(reservasT);
            }
            foreach (ReservaInsModel reserva in reservasIns)
            {
                ReservasResponse reservasT = new ReservasResponse();

                reservasT.ID = reserva.ID;
                reservasT.Tipo = reserva.Tipo;   
                reservasT.Desc = reserva.Dia + " " + reserva.Hora;
                reservasU.Add(reservasT);
            }
            if (reservasU == null)
                return NotFound();
            return Ok(reservasU);
        }

        [HttpPost(template: ApiRoutes.Reserva.ReservarInstalacion)]
        public async Task<ActionResult<string>> ReservarInstalacion(ReservaRequest request)
        {
            try
            {
                if (request.Tipo == "Coliseo" || request.Tipo == "Cancha")
                {
                    HorarioModel horarioT = await context.Horarios.Where(i => i.Dia == request.Dia).Where(i=>i.Desc == request.Hora).FirstOrDefaultAsync();
                    if(horarioT != null)
                    {
                        if (request.Tipo == "Coliseo")
                            horarioT.Coliseo = false;
                        if (request.Tipo == "Cancha")
                            horarioT.Cancha = false;

                        context.Horarios.Update(horarioT);
                        ReservaInsModel reserva = new ReservaInsModel();
                        reserva.Dia = horarioT.Dia;
                        reserva.Hora = horarioT.Desc;
                        reserva.Tipo = request.Tipo;
                        reserva.Hor_ID = horarioT.ID;
                        reserva.User_ID = request.UsuarioID;

                        var created = context.ReservaInstalaciones.Add(reserva);
                        await context.SaveChangesAsync();
                        return Ok();
                    }
                    return BadRequest("No se encontro el horario"); 
                }

                return BadRequest("No es Cancha o Coliseo");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(template: ApiRoutes.Reserva.ReservarImplemento)]
        public async Task<ActionResult<string>> ReservarImplento(ReservaRequest request)
        {
            try
            {
                ImplementoModel implementoT = await context.Implementos.Where(i => i.Desc == request.Tipo).FirstOrDefaultAsync();

                ReservaImpModel reserva = new ReservaImpModel();

                reserva.Dia = request.Dia;
                reserva.Tipo = implementoT.Desc;
                reserva.Imp_ID = implementoT.ID; ;
                reserva.User_ID = request.UsuarioID;

                var created = context.ReservaImplementos.Add(reserva);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
