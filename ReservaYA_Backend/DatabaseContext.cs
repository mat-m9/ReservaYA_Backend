using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservaYA_Backend.Models;
using ReservaYA_Backend.ResponseModels;

namespace ReservaYA_Backend
{
    public partial class DatabaseContext : IdentityDbContext<UserModel>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
        }

        public virtual DbSet<UserModel> Usuarios { get; set; } = null!;
        public virtual DbSet<ReservaImpModel> ReservaImplementos { get; set; } = null!;
        public virtual DbSet<ReservaInsModel> ReservaInstalaciones { get; set; } = null!;
        public virtual DbSet<HorarioModel> Horarios { get; set; } = null!;
        public virtual DbSet<ImplementoModel> Implementos { get; set; } = null!;

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("uuid-ossp");
            //builder.Entity<ReservaImpModel>()
            //     .HasIndex(x => x.Imp_ID)
            //     .IsUnique();

            //builder.Entity<ReservaInsModel>()
            //     .HasIndex(x => x.Hor_ID)
            //     .IsUnique();

            builder.Entity<ReservaImpModel>(o =>
                o.Property(x => x.ID)
                .HasDefaultValue("uuid_generate_v4()")
                .ValueGeneratedOnAdd());
            builder.Entity<ReservaInsModel>(o =>
                o.Property(x => x.ID)
                .HasDefaultValue("uuid_generate_v4()")
                .ValueGeneratedOnAdd());
            builder.Entity<ImplementoModel>(o =>
                o.Property(x => x.ID)
                .HasDefaultValue("uuid_generate_v4()")
                .ValueGeneratedOnAdd());
            builder.Entity<HorarioModel>(o =>
                o.Property(x => x.ID)
                .HasDefaultValue("uuid_generate_v4()")
                .ValueGeneratedOnAdd());

            base.OnModelCreating(builder);
        }
    }
}
