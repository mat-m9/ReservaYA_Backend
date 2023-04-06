using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ReservaYA_Backend.Services
{
    public class PopulateDB
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                DatabaseContext context= scope.ServiceProvider.GetService<DatabaseContext>();

                DefaultRolesService rolesService= scope.ServiceProvider.GetRequiredService<DefaultRolesService>();

                string[,] horarios = new string[,]
                {
                    {"Lunes", "07:00 - 08:00"},
                    {"Lunes", "08:00 - 09:00"},
                    {"Lunes", "09:00 - 10:00"},
                    {"Lunes", "10:00 - 11:00"},
                    {"Lunes", "11:00 - 12:00"},
                    {"Lunes", "12:00 - 13:00"},
                    {"Lunes", "13:00 - 14:00"},
                    {"Lunes", "14:00 - 15:00"},
                    {"Lunes", "15:00 - 16:00"},
                    {"Lunes", "16:00 - 17:00"},
                    {"Lunes", "17:00 - 18:00"},
                    {"Lunes", "18:00 - 19:00"},
                    {"Lunes", "19:00 - 20:00"},
                    {"Lunes", "20:00 - 21:00"},
                    {"Martes", "07:00 - 08:00"},
                    {"Martes", "08:00 - 09:00"},
                    {"Martes", "09:00 - 10:00"},
                    {"Martes", "10:00 - 11:00"},
                    {"Martes", "11:00 - 12:00"},
                    {"Martes", "12:00 - 13:00"},
                    {"Martes", "13:00 - 14:00"},
                    {"Martes", "14:00 - 15:00"},
                    {"Martes", "15:00 - 16:00"},
                    {"Martes", "16:00 - 17:00"},
                    {"Martes", "17:00 - 18:00"},
                    {"Martes", "18:00 - 19:00"},
                    {"Martes", "19:00 - 20:00"},
                    {"Martes", "20:00 - 21:00"},
                    {"Miercoles", "07:00 - 08:00"},
                    {"Miercoles", "08:00 - 09:00"},
                    {"Miercoles", "09:00 - 10:00"},
                    {"Miercoles", "10:00 - 11:00"},
                    {"Miercoles", "11:00 - 12:00"},
                    {"Miercoles", "12:00 - 13:00"},
                    {"Miercoles", "13:00 - 14:00"},
                    {"Miercoles", "14:00 - 15:00"},
                    {"Miercoles", "15:00 - 16:00"},
                    {"Miercoles", "16:00 - 17:00"},
                    {"Miercoles", "17:00 - 18:00"},
                    {"Miercoles", "18:00 - 19:00"},
                    {"Miercoles", "19:00 - 20:00"},
                    {"Miercoles", "20:00 - 21:00"},
                    {"Jueves", "07:00 - 08:00"},
                    {"Jueves", "08:00 - 09:00"},
                    {"Jueves", "09:00 - 10:00"},
                    {"Jueves", "10:00 - 11:00"},
                    {"Jueves", "11:00 - 12:00"},
                    {"Jueves", "12:00 - 13:00"},
                    {"Jueves", "13:00 - 14:00"},
                    {"Jueves", "14:00 - 15:00"},
                    {"Jueves", "15:00 - 16:00"},
                    {"Jueves", "16:00 - 17:00"},
                    {"Jueves", "17:00 - 18:00"},
                    {"Jueves", "18:00 - 19:00"},
                    {"Jueves", "19:00 - 20:00"},
                    {"Jueves", "20:00 - 21:00"},
                    {"Viernes", "07:00 - 08:00"},
                    {"Viernes", "08:00 - 09:00"},
                    {"Viernes", "09:00 - 10:00"},
                    {"Viernes", "10:00 - 11:00"},
                    {"Viernes", "11:00 - 12:00"},
                    {"Viernes", "12:00 - 13:00"},
                    {"Viernes", "13:00 - 14:00"},
                    {"Viernes", "14:00 - 15:00"},
                    {"Viernes", "15:00 - 16:00"},
                    {"Viernes", "16:00 - 17:00"},
                    {"Viernes", "17:00 - 18:00"},
                    {"Viernes", "18:00 - 19:00"},
                    {"Viernes", "19:00 - 20:00"},
                    {"Viernes", "20:00 - 21:00"}
                };
                Dictionary<string, int> implementos = new Dictionary<string, int>()
                {
                    {"Balones de futbol", 10},
                    {"Balones de basquet",15},
                    {"Raquetas de Ping Pong",7}
                };

                List<string> roles = rolesService.GetRolesList();
                var roleStore = new RoleStore<IdentityRole>(context);
                foreach (string role in roles)
                {
                    if (!context.Roles.Any(r => r.NormalizedName == role.ToUpper()))
                    {
                        await roleStore.CreateAsync(new()
                        {
                            Name = role,
                            NormalizedName = role.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        });
                    }
                }

                var user = new UserModel()
                {
                    UserName = "administrador",
                    NormalizedUserName = "ADMINISTRADOR",
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var rol = await roleStore.FindByNameAsync(await rolesService.GetDefaultRole("administrador"));

                var foundh = await context.Horarios.FirstOrDefaultAsync();
                for (int i = 0; i < horarios.Length -1; i++)
                {
                    
                    if (foundh == null)
                    {
                        context.Horarios.Add(new Models.HorarioModel
                        {
                            Dia = horarios[i,0],
                            Desc = horarios[i, 1]
                        });
                        await context.SaveChangesAsync();
                    }
                }
                foreach (KeyValuePair<string, int> implemento in implementos)
                {
                    var found = await context.Implementos.Where(i => i.Desc.Equals(implemento.Key)).FirstOrDefaultAsync();
                    if (found == null)
                    {
                        context.Implementos.Add(new Models.ImplementoModel
                        {
                            Desc = implemento.Key,
                            Cant = implemento.Value
                        });
                        await context.SaveChangesAsync();
                    }
                }
                UserManager<UserModel> _userManager = scope.ServiceProvider.GetService<UserManager<UserModel>>();
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<UserModel>();
                    var hashed = password.HashPassword(user, "P!1admin");
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<UserModel>(context);
                    var result = await userStore.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, rol.NormalizedName);
                }
                else
                {
                    var user1 = await context.Users.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();
                    await _userManager.AddToRoleAsync(user1, rol.Name);
                }
            }
        }
    }
}
