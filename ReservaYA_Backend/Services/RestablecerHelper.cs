using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Controllers;
using ReservaYA_Backend.Models;

namespace ReservaYA_Backend.Services
{
    public class RestablecerHelper
    {
        private readonly DatabaseContext context;



        private ReservasController? reservasController;

        public RestablecerHelper(DatabaseContext context)
        {
            this.context = context;
        }

        public async void Restablecer()
        {
            UserModel admin = await context.Users.Where(u => u.UserName == "administrador").FirstOrDefaultAsync();
            var reservas = await context.ReservaInstalaciones.Where(r => r.User_ID != admin.Id).ToListAsync();

            foreach (var reservation in reservas)
            {
                await reservasController.Delete(reservation.ID);
            }

       
    }
}
}
