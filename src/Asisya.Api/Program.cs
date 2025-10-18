using Asisya.Application;
using Asisya.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asisya API", Version = "v1" });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
});
builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", policy => { policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); }));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT
var key = builder.Configuration["Jwt:Key"] ?? "this_is_a_dev_key_with_more_than_32_chars_123456";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "Asisya";
var audience = builder.Configuration["Jwt:Audience"] ?? "AsisyaClients";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = signingKey
  };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply EnsureCreated and seed SERVIDORES/CLOUD
await app.Services.ApplyMigrationsAndSeedAsync();

app.Run();
