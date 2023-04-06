namespace ReservaYA_Backend.Services
{
    public class DefaultRolesService
    {
        public static readonly Dictionary<string, string> Roles = new Dictionary<string, string>
        {
            {"administrador","ADMINISTRADOR" },
            {"usuario", "USUARIO" }
        };

        public Task<string> GetDefaultRole(string key)
        {
            return Task.FromResult(Roles.ContainsKey(key) ? Roles[key] : null);
        }

        public List<string> GetRolesList()
        {
            return Roles.Values.ToList();
        }
    }
}
