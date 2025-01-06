using Catedra3IDWMBackend.src.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();

string connectionStringDB = Environment.GetEnvironmentVariable("DATA_BASE_URL") ?? "Data Source=app.db";
builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseSqlite(connectionStringDB));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();