using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEscuela.Repository.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosInscripcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosInscripcion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carreras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Sigla = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DuracionAnios = table.Column<int>(type: "INTEGER", nullable: false),
                    TituloOtorgado = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PreceptorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carreras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Dni = table.Column<int>(type: "INTEGER", maxLength: 20, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime", nullable: false),
                    TipoPersona = table.Column<string>(type: "TEXT", nullable: false),
                    CarreraId = table.Column<int>(type: "INTEGER", nullable: true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_Carreras_CarreraId",
                        column: x => x.CarreraId,
                        principalTable: "Carreras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    Cuatrimestre = table.Column<int>(type: "INTEGER", nullable: false),
                    CupoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    CarreraId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocenteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materias_Carreras_CarreraId",
                        column: x => x.CarreraId,
                        principalTable: "Carreras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materias_Personas_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CorreoElectronico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Dni = table.Column<int>(type: "INTEGER", maxLength: 20, nullable: false),
                    Contrasenia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RolId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonaId = table.Column<int>(type: "INTEGER", nullable: false),
                    RolId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId1",
                        column: x => x.RolId1,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    AlumnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    MateriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstadoInscripcionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => new { x.AlumnoId, x.MateriaId });
                    table.ForeignKey(
                        name: "FK_Inscripciones_EstadosInscripcion_EstadoInscripcionId",
                        column: x => x.EstadoInscripcionId,
                        principalTable: "EstadosInscripcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Personas_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EstadosInscripcion",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Pendiente" });

            migrationBuilder.InsertData(
                table: "EstadosInscripcion",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 2, "Aceptada" });

            migrationBuilder.InsertData(
                table: "EstadosInscripcion",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 3, "Rechazada" });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "CorreoElectronico", "Dni", "FechaNacimiento", "NombreCompleto", "TipoPersona" },
                values: new object[] { 1, "admin@escuela.com", 12345678, new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador Principal", "Base" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Administrador" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 2, "Alumno" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 3, "Docente" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 4, "Preceptor" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 5, "Invitado" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Contrasenia", "CorreoElectronico", "Dni", "PersonaId", "RolId", "RolId1" },
                values: new object[] { 1, "12345678", "admin@escuela.com", 12345678, 1, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_Carreras_PreceptorId",
                table: "Carreras",
                column: "PreceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_EstadoInscripcionId",
                table: "Inscripciones",
                column: "EstadoInscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_MateriaId",
                table: "Inscripciones",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_CarreraId",
                table: "Materias",
                column: "CarreraId");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_DocenteId",
                table: "Materias",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CarreraId",
                table: "Personas",
                column: "CarreraId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PersonaId",
                table: "Usuarios",
                column: "PersonaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId1",
                table: "Usuarios",
                column: "RolId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Carreras_Usuarios_PreceptorId",
                table: "Carreras",
                column: "PreceptorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carreras_Usuarios_PreceptorId",
                table: "Carreras");

            migrationBuilder.DropTable(
                name: "Inscripciones");

            migrationBuilder.DropTable(
                name: "EstadosInscripcion");

            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Carreras");
        }
    }
}
