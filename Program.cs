using System.Text;
using Catedra3IDWMBackend.src.Data;
using Catedra3IDWMBackend.src.Interfaces;
using Catedra3IDWMBackend.src.Middleware;
using Catedra3IDWMBackend.src.Models;
using Catedra3IDWMBackend.src.Repositories;
using Catedra3IDWMBackend.src.Services;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.WithOrigins("http://localhost:4200", "http://localhost:8100")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});


Env.Load();

string CloudinaryName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME") ?? throw new ArgumentNullException("El nombre de la cuenta de Cloudinary no puede ser nulo o vacío.");
string CloudinaryApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? throw new ArgumentNullException("La clave de la API de Cloudinary no puede ser nula o vacía.");
string CloudinaryApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? throw new ArgumentNullException("El secreto de la API de Cloudinary no puede ser nulo o vacío.");

    var CloudinaryAccount = new Account(
        CloudinaryName,
        CloudinaryApiKey,
        CloudinaryApiSecret
    );
var Cloudinary = new Cloudinary(CloudinaryAccount);
builder.Services.AddSingleton(Cloudinary);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPost, PostRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityCore<User>(
    opt => {
        opt.Password.RequireDigit = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 8;
    }
).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme =
    opt.DefaultChallengeScheme =
    opt.DefaultForbidScheme = 
    opt.DefaultScheme =
    opt.DefaultSignInScheme =
    opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? throw new ArgumentNullException("Clave de firma no puede ser nula o vacía"))),
    };
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

string connectionStringDB = Environment.GetEnvironmentVariable("DATA_BASE_URL") ?? "Data Source=app.db";
builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseSqlite(connectionStringDB));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseMiddleware<BlacklistMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();