using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Models;

namespace WebEscuela.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<EstadoInscripcion> EstadosInscripcion { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Docente> Docentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inscripcion (AlumnoId + MateriaId)
            modelBuilder.Entity<Inscripcion>()
                .HasKey(i => new { i.AlumnoId, i.MateriaId });

            // MATERIA
            modelBuilder.Entity<Materia>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Materia>()
                .Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Materia>()
                .HasOne<Carrera>()
                .WithMany()
                .HasForeignKey(m => m.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Materia>()
                .HasOne(m => m.Docente)
                .WithMany(d => d.Materias)
                .HasForeignKey(m => m.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);


            // USUARIO
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Dni)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Contrasenia)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Persona)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(u => u.PersonaId)
                .OnDelete(DeleteBehavior.Restrict);


            // ALUMNO

            modelBuilder.Entity<Alumno>()
                .HasOne<Carrera>()
                .WithMany()
                .HasForeignKey(a => a.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // INSCRIPCION
            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Alumno)
                .WithMany(a => a.Inscripciones)
                .HasForeignKey(i => i.AlumnoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Materia)
                .WithMany(m => m.Inscripciones)
                .HasForeignKey(i => i.MateriaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.EstadoInscripcion)
                .WithMany(e => e.Inscripciones)
                .HasForeignKey(i => i.EstadoInscripcionId)
                .OnDelete(DeleteBehavior.Restrict);


            // DOCENTE

            modelBuilder.Entity<Docente>()
                .Property(d => d.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            // CARRERA
            modelBuilder.Entity<Carrera>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Carrera>()
                .Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Carrera>()
                .Property(c => c.Sigla)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Carrera>()
                .Property(c => c.TituloOtorgado)
                .IsRequired()
                .HasMaxLength(150);
            modelBuilder.Entity<Carrera>()
                .HasOne(c => c.Preceptor)
                .WithMany()
                .HasForeignKey(c => c.PreceptorId)
                .OnDelete(DeleteBehavior.Restrict);


            // PERSONA
            modelBuilder.Entity<Persona>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Persona>()
                .HasDiscriminator<string>("TipoPersona")
                .HasValue<Persona>("Base")
                .HasValue<Alumno>("Alumno")
                .HasValue<Docente>("Docente");


            modelBuilder.Entity<Persona>()
                .Property(p => p.NombreCompleto)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Dni)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Persona>()
                .Property(p => p.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.FechaNacimiento)
                .IsRequired()
                .HasColumnType("datetime");


            // ROL
            modelBuilder.Entity<Rol>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Rol>()
                .Property(r => r.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            // ESTADO INSCRIPCION
            modelBuilder.Entity<EstadoInscripcion>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<EstadoInscripcion>()
                .Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50);


            // DATOS BASICOS

            // Seed Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Alumno" },
                new Rol { Id = 3, Nombre = "Docente" },
                new Rol { Id = 4, Nombre = "Preceptor" },
                new Rol { Id = 5, Nombre = "Invitado" }
            );

            // Seed Estados de inscripción
            modelBuilder.Entity<EstadoInscripcion>().HasData(
                new EstadoInscripcion { Id = 1, Nombre = "Pendiente" },
                new EstadoInscripcion { Id = 2, Nombre = "Aceptada" },
                new EstadoInscripcion { Id = 3, Nombre = "Rechazada" }
            );

            // Seed Persona admin
            modelBuilder.Entity<Persona>().HasData(
                new Persona
                {
                    Id = 1,
                    NombreCompleto = "Administrador Principal",
                    Dni = 12345678,
                    CorreoElectronico = "admin@escuela.com",
                    FechaNacimiento = new DateTime(1980, 1, 1)
                }
            );

            // Seed Usuario admin
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    CorreoElectronico = "admin@escuela.com",
                    Dni = 12345678,
                    Contrasenia = "12345678",
                    RolId = 1,
                    PersonaId = 1
                }
            );
        }
    }
}

// Hacer migracion : dotnet ef migrations add Inicial --project WebEscuela.Repository --startup-project WebEscuela
// Crear base de datos : dotnet ef database update
