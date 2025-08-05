using Microsoft.EntityFrameworkCore;
using webapi.event_.Domains;

namespace webapi.event_.Contexts
{
    public class Context : DbContext
    {
        public Context()
        {
        }
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<TiposUsuarios> TiposUsuarios { get; set; }

        public DbSet<Usuarios> Usuarios { get; set; }

        public DbSet<TiposEventos> TiposEventos { get; set; }

        public DbSet<Eventos> Eventos { get; set; }

        public DbSet<ComentariosEventos> ComentariosEventos { get; set; }
       
        public DbSet<Instituicoes> Instituicoes { get; set; }

        public DbSet<PresencasEventos> PresencasEventos { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=DESKTOP-LAO5MIJ\\SQLEXPRESSTEC; Database=event; User Id = sa; Pwd=abc123; TrustServerCertificate=true;");
                optionsBuilder.UseSqlServer("Server=tcp:serverdbevent.database.windows.net,1433;Initial Catalog=event_db;Persist Security Info=False;User ID=eventdb;Password=Senai@134;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                
            }
        }
    }
}