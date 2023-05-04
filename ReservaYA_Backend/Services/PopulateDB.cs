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
                    {"Lunes", "7"},
                    {"Lunes", "8"},
                    {"Lunes", "9"},
                    {"Lunes", "10"},
                    {"Lunes", "11"},
                    {"Lunes", "12"},
                    {"Lunes", "13"},
                    {"Lunes", "14"},
                    {"Lunes", "15"},
                    {"Lunes", "16"},
                    {"Lunes", "17"},
                    {"Lunes", "18"},
                    {"Lunes", "19"},
                    {"Lunes", "20"},
                    {"Martes", "7"},
                    {"Martes", "8"},
                    {"Martes", "9"},
                    {"Martes", "10"},
                    {"Martes", "11"},
                    {"Martes", "12"},
                    {"Martes", "13"},
                    {"Martes", "14"},
                    {"Martes", "15"},
                    {"Martes", "16"},
                    {"Martes", "17"},
                    {"Martes", "18"},
                    {"Martes", "19"},
                    {"Martes", "20"},
                    {"Miercoles", "7"},
                    {"Miercoles", "8"},
                    {"Miercoles", "9"},
                    {"Miercoles", "10"},
                    {"Miercoles", "11"},
                    {"Miercoles", "12"},
                    {"Miercoles", "13"},
                    {"Miercoles", "14"},
                    {"Miercoles", "15"},
                    {"Miercoles", "16"},
                    {"Miercoles", "17"},
                    {"Miercoles", "18"},
                    {"Miercoles", "19"},
                    {"Miercoles", "20"},
                    {"Jueves", "7"},
                    {"Jueves", "8"},
                    {"Jueves", "9"},
                    {"Jueves", "10"},
                    {"Jueves", "11"},
                    {"Jueves", "12"},
                    {"Jueves", "13"},
                    {"Jueves", "14"},
                    {"Jueves", "15"},
                    {"Jueves", "16"},
                    {"Jueves", "17"},
                    {"Jueves", "18"},
                    {"Jueves", "19"},
                    {"Jueves", "20"},
                    {"Viernes", "7"},
                    {"Viernes", "8"},
                    {"Viernes", "9"},
                    {"Viernes", "10"},
                    {"Viernes", "11"},
                    {"Viernes", "12"},
                    {"Viernes", "13"},
                    {"Viernes", "14"},
                    {"Viernes", "15"},
                    {"Viernes", "16"},
                    {"Viernes", "170"},
                    {"Viernes", "18"},
                    {"Viernes", "19"},
                    {"Viernes", "20"}
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
