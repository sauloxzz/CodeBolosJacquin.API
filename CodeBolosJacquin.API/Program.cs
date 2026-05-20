using CodeBolosJacquin.API.Context;
using CodeBolosJacquin.API.Interfaces;
using CodeBolosJacquin.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

//pegando a connectionString 
var coonnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Registrando o DbContex
builder.Services.AddDbContext<BolosJacquinContext>(
    options => options.UseSqlServer(coonnectionString));


// Registrando as dependências (injeção de dependências)
builder.Services.AddScoped<IBoloRepository, BoloRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add autenticação com Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Coloque seu Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Ativar a exibição do Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
