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
        public DbSet<Carrera> Carreras { get; set; } = null!;
        public DbSet<Modalidad> Modalidades { get; set; } = null!;
        public DbSet<Turno> Turnos { get; set; } = null!;
        public DbSet<Materia> Materias { get; set; } = null!;
        public DbSet<Persona> Personas { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<EstadoInscripcion> EstadosInscripcion { get; set; } = null!;
        public DbSet<Inscripcion> Inscripciones { get; set; } = null!;
        public DbSet<Alumno> Alumnos { get; set; } = null!;
        public DbSet<Docente> Docentes { get; set; } = null!;
        public DbSet<TipoCursado> TiposCursado { get; set; } = null!;

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
                .HasOne(m => m.Carrera)
                .WithMany(c => c.Materias)
                .HasForeignKey(m => m.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Materia>()
                .HasOne(m => m.Docente)
                .WithMany(d => d.Materias)
                .HasForeignKey(m => m.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);



            // USUARIO
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Contrasenia)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Persona)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(u => u.PersonaId)
                .OnDelete(DeleteBehavior.Cascade);


            // ALUMNO

            modelBuilder.Entity<Alumno>()
                .HasOne(a => a.Carrera)
                .WithMany(c => c.Alumnos)
                .HasForeignKey(a => a.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // INSCRIPCION
            modelBuilder.Entity<Inscripcion>()
                .HasKey(i => new { i.AlumnoId, i.MateriaId });

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Alumno)
                .WithMany(a => a.Inscripciones)
                .HasForeignKey(i => i.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Materia)
                .WithMany(m => m.Inscripciones)
                .HasForeignKey(i => i.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.TipoCursado)
                .WithMany(tc => tc.Inscripciones)
                .HasForeignKey(i => i.TipoCursadoId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasIndex(c => c.Sigla)
                .IsUnique();

            modelBuilder.Entity<Carrera>()
                .Property(c => c.TituloOtorgado)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Carrera>()
                .HasOne(c => c.Modalidad)
                .WithMany(m => m.Carreras)
                .HasForeignKey(c => c.ModalidadId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Carrera>()
                .HasOne(c => c.Turno)
                .WithMany(t => t.Carreras)
                .HasForeignKey(c => c.TurnoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Carrera>()
                .HasOne(c => c.Preceptor)
                .WithMany()
                .HasForeignKey(c => c.PreceptorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TIPOCURSADO
            modelBuilder.Entity<TipoCursado>()
                .HasKey(tp => tp.Id);

            modelBuilder.Entity<TipoCursado>()
                .Property(tp => tp.Nombre)
                .IsRequired()
                .HasMaxLength(50);


            // MODALIDAD
            modelBuilder.Entity<Modalidad>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Modalidad>()
                .Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            // TURNO
            modelBuilder.Entity<Turno>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Turno>()
                .Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            // PERSONA
            modelBuilder.Entity<Persona>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

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
                .HasIndex(p => p.Dni)
                .IsUnique();

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

            // Seed TipoCursado
            modelBuilder.Entity<TipoCursado>().HasData(
                new TipoCursado { Id = 1, Nombre = "Regular" },
                new TipoCursado { Id = 2, Nombre = "Libre" }
);

            // Seed Modalidades
            modelBuilder.Entity<Modalidad>().HasData(
                new Modalidad { Id = 1, Nombre = "Presencial" },
                new Modalidad { Id = 2, Nombre = "Virtual" },
                new Modalidad { Id = 3, Nombre = "Semipresencial" }
            );

            // Seed Turnos
            modelBuilder.Entity<Turno>().HasData(
                new Turno { Id = 1, Nombre = "Matutina" },
                new Turno { Id = 2, Nombre = "Vespertina" },
                new Turno { Id = 3, Nombre = "Nocturna" },
                new Turno { Id = 4, Nombre = "Diurna" }
            );

            // Seed Persona admin
            modelBuilder.Entity<Persona>().HasData(
                new Persona
                {
                    Id = 1,
                    NombreCompleto = "Administrador Principal",
                    Dni = "12345678",
                    CorreoElectronico = "admin@escuela.com",
                    FechaNacimiento = new DateTime(1980, 1, 1)
                }
            );

            // Seed Usuario admin
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Contrasenia = "12345678",
                    RolId = 1,
                    PersonaId = 1
                }
            );
        }
    }
}

// Hacer migracion : dotnet ef migrations add Inicial --project WebEscuela.Repository --startup-project WebEscuela
// Crear base de datos : dotnet ef database update --project WebEscuela.Repository --startup-project WebEscuela
