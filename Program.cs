using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlite("DataSource=biblioteca.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biblioteca API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1"));

app.MapGet("/bibliotecas", async (BibliotecaContext db) => await db.Bibliotecas.ToListAsync());

app.MapGet("/bibliotecas/{id}", async (int id, BibliotecaContext db) =>
    await db.Bibliotecas.FindAsync(id) is Biblioteca biblioteca ? Results.Ok(biblioteca) : Results.NotFound());

app.MapPost("/bibliotecas", async (Biblioteca biblioteca, BibliotecaContext db) =>
{
    db.Bibliotecas.Add(biblioteca);
    await db.SaveChangesAsync();
    return Results.Created($"/bibliotecas/{biblioteca.Id}", biblioteca);
});

app.MapPut("/bibliotecas/{id}", async (int id, Biblioteca bibliotecaAtualizada, BibliotecaContext db) =>
{
    var biblioteca = await db.Bibliotecas.FindAsync(id);
    if (biblioteca is null) return Results.NotFound();

    biblioteca.Nome = bibliotecaAtualizada.Nome;
    biblioteca.InicioFuncionamento = bibliotecaAtualizada.InicioFuncionamento;
    biblioteca.FinalFuncionamento = bibliotecaAtualizada.FinalFuncionamento;
    biblioteca.Inauguracao = bibliotecaAtualizada.Inauguracao;
    biblioteca.Contato = bibliotecaAtualizada.Contato;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/bibliotecas/{id}", async (int id, BibliotecaContext db) =>
{
    var biblioteca = await db.Bibliotecas.FindAsync(id);
    if (biblioteca is null) return Results.NotFound();

    db.Bibliotecas.Remove(biblioteca);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
