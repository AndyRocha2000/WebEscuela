using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using Scrutor;
using WebEscuela.Service.Interfaces;
using WebEscuela.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexion a base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorPages();

// Agregar los servicios
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(PersonaService))
    .AddClasses(classes => classes.InNamespaces("WebEscuela.Service.Services"))
    .AsMatchingInterface()
    .WithScopedLifetime());

// Agregamos autenticacion con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Página donde irá si no está logueado
        options.AccessDeniedPath = "/AccessDenied"; // Página para acceso denegado (opcional)
    });

// Agregamos la autenticacion
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
