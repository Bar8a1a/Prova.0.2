using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados (SQLite)

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=consumo_agua_Barbara.db"));

// 2. Configuração do CORS (Para o Front acessar o Back)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AcessoTotal",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AcessoTotal");

// --- ROTAS (ENDPOINTS) ---

// Funcionalidade 1: Cadastrar
app.MapPost("/api/consumo/cadastrar", async (AppDbContext db, [FromBody] RegistroConsumoAgua registro) =>
{
    // Lógica de Negócio
    registro.ConsumoIdeal = registro.Peso * 35;
    registro.Classificacao = ClassificarConsumo(registro.ConsumoIdeal);
    registro.DataCriacao = DateTime.Now;

    db.Registros.Add(registro);
    await db.SaveChangesAsync();

    return Results.Created($"/api/consumo/{registro.Id}", registro);
});

// Funcionalidade 2: Listar Todos
app.MapGet("/api/consumo/listar", async (AppDbContext db) =>
{
    var lista = await db.Registros.OrderByDescending(x => x.DataCriacao).ToListAsync();
    return Results.Ok(lista);
});

// Funcionalidade 3: Listar por Status
app.MapGet("/api/consumo/listarporstatus/{classificacao}", async (AppDbContext db, string classificacao) =>
{
    // Opcional: decodificar URL caso venha com espaços estranhos, mas o EF lida bem
    var lista = await db.Registros
        .Where(x => x.Classificacao.ToLower() == classificacao.ToLower())
        .ToListAsync();
    
    return Results.Ok(lista);
});

// Funcionalidade 4: Alterar Peso
app.MapPut("/api/consumo/alterar/{id}", async (AppDbContext db, int id, [FromBody] AtualizarPesoRequest dados) =>
{
    var registro = await db.Registros.FindAsync(id);
    if (registro == null) return Results.NotFound();

    // Atualiza dados
    registro.Peso = dados.Peso;
    registro.ConsumoIdeal = registro.Peso * 35;
    registro.Classificacao = ClassificarConsumo(registro.ConsumoIdeal);

    await db.SaveChangesAsync();
    return Results.Ok(registro);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

// --- MÉTODOS AUXILIARES ---
string ClassificarConsumo(double consumo)
{
    if (consumo < 1500) return "Baixo consumo";
    if (consumo <= 2500) return "Consumo adequado"; // 1500 a 2500
    return "Alto consumo"; // Acima de 2500
}

// --- CLASSES E MODELOS ---

public class RegistroConsumoAgua
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public double Peso { get; set; }
    public double ConsumoIdeal { get; set; }
    public string? Classificacao { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class AtualizarPesoRequest
{
    public double Peso { get; set; }
}

// Contexto do Banco
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<RegistroConsumoAgua> Registros { get; set; }
}
