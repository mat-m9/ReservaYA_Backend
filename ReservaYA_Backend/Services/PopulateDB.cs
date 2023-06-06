using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using static ReservaYA_Backend.ApiRoutes;

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
                    {"Miércoles", "7"},
                    {"Miércoles", "8"},
                    {"Miércoles", "9"},
                    {"Miércoles", "10"},
                    {"Miércoles", "11"},
                    {"Miércoles", "12"},
                    {"Miércoles", "13"},
                    {"Miércoles", "14"},
                    {"Miércoles", "15"},
                    {"Miércoles", "16"},
                    {"Miércoles", "17"},
                    {"Miércoles", "18"},
                    {"Miércoles", "19"},
                    {"Miércoles", "20"},
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
                    {"Viernes", "17"},
                    {"Viernes", "18"},
                    {"Viernes", "19"},
                    {"Viernes", "20"}
                };
                Dictionary<string, int> implementos = new Dictionary<string, int>()
                {
                    {"Balones Futbol", 20},
                    {"Balones Basquet",17},
                    {"Balones Volley",15},
                    {"Raquetas PinPon",20},
                    {"Chalecos",40}
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
                for (int i = 0; i < horarios.GetLength(0); i++)
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



                string[,] reservasFijas = new string[,]
                {
                    {"Lunes", "Coliseo", "7" },
                    {"Lunes", "Coliseo", "8" },
                    {"Lunes", "Coliseo", "9" },
                    {"Lunes", "Coliseo", "10" },
                    {"Lunes", "Coliseo", "11" },
                    {"Lunes", "Coliseo", "12" },
                    {"Lunes", "Coliseo", "16" },
                    {"Lunes", "Coliseo", "17" },
                    {"Lunes", "Coliseo", "18" },
                    {"Lunes", "Coliseo", "19" },
                    {"Lunes", "Coliseo", "20" },

                    {"Martes", "Coliseo", "12" },
                    {"Martes", "Coliseo", "13" },
                    {"Martes", "Coliseo", "14" },
                    {"Martes", "Coliseo", "15" },
                    {"Martes", "Coliseo", "16" },
                    {"Martes", "Coliseo", "17" },
                    {"Martes", "Coliseo", "18" },
                    {"Martes", "Coliseo", "19" },
                    {"Martes", "Coliseo", "20" },


                    {"Miércoles", "Coliseo", "7" },
                    {"Miércoles", "Coliseo", "8" },
                    {"Miércoles", "Coliseo", "9" },
                    {"Miércoles", "Coliseo", "10" },
                    {"Miércoles", "Coliseo", "11" },
                    {"Miércoles", "Coliseo", "12" },
                    {"Miércoles", "Coliseo", "13" },
                    {"Miércoles", "Coliseo", "14" },
                    {"Miércoles", "Coliseo", "16" },
                    {"Miércoles", "Coliseo", "17" },
                    {"Miércoles", "Coliseo", "18" },
                    {"Miércoles", "Coliseo", "19" },
                    {"Miércoles", "Coliseo", "20" },

                    {"Jueves", "Coliseo", "9" },
                    {"Jueves", "Coliseo", "10" },
                    {"Jueves", "Coliseo", "11" },
                    {"Jueves", "Coliseo", "12" },
                    {"Jueves", "Coliseo", "13" },
                    {"Jueves", "Coliseo", "14" },
                    {"Jueves", "Coliseo", "15" },
                    {"Jueves", "Coliseo", "16" },
                    {"Jueves", "Coliseo", "18" },
                    {"Jueves", "Coliseo", "19" },
                    {"Jueves", "Coliseo", "20" },

                    {"Viernes", "Coliseo", "7" },
                    {"Viernes", "Coliseo", "8" },
                    {"Viernes", "Coliseo", "18" },
                    {"Viernes", "Coliseo", "19" },
                    {"Viernes", "Coliseo", "20" },


                    {"Lunes", "Cancha", "17" },
                    {"Lunes", "Cancha", "18" },
                    {"Lunes", "Cancha", "19" },
                    {"Lunes", "Cancha", "20" },


                    {"Martes", "Cancha", "17" },
                    {"Martes", "Cancha", "18" },
                    {"Martes", "Cancha", "19" },
                    {"Martes", "Cancha", "20" },

                    {"Miércoles", "Cancha", "17" },
                    {"Miércoles", "Cancha", "18" },
                    {"Miércoles", "Cancha", "19" },
                    {"Miércoles", "Cancha", "20" },

                    {"Jueves", "Cancha", "17" },
                    {"Jueves", "Cancha", "18" },
                    {"Jueves", "Cancha", "19" },
                    {"Jueves", "Cancha", "20" }
                };

                UserModel admin =  await context.Users.Where(u => u.UserName == "administrador").FirstOrDefaultAsync();

                var foundI = await context.ReservaInstalaciones.FirstOrDefaultAsync();
                for (int i = 0; i < reservasFijas.GetLength(0); i++)
                {
                    if (foundI == null)
                    {
                        var horario = await context.Horarios.Where(h => h.Dia == reservasFijas[i,0]).Where(h => h.Desc == reservasFijas[i,2]).FirstOrDefaultAsync();
                        if (reservasFijas[i, 1] == "Cancha")
                            horario.Cancha = false;
                        else
                            horario.Coliseo = false;

                        context.ReservaInstalaciones.Add(new Models.ReservaInsModel
                        {
                            Dia =  horario.Dia,
                            Tipo = reservasFijas[i, 1],
                            Hora = horario.Desc,
                            Hor_ID = horario.ID,
                            User_ID = admin.Id
                        });
                        context.Horarios.Update(horario);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
