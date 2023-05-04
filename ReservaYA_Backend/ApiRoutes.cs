namespace ReservaYA_Backend
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Base = Root;

        public static class Horario
        {
            public const string Cancha = "canchaDisponibles";
            public const string Coliseo = "coliseoDisponibles";
        }
        public static class Reserva
        {
            public const string ReservarImplemento = "reservarImplemento";
            public const string ReservarInstalacion = "reservarInstalacion";
        }
        public static class User
        {
            public const string Register = "register";
            public const string Login = "login";
            public const string Change = "change";
        }
    }
}
