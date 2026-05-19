using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using rpa_data_collector.Application.Services;
using rpa_data_collector.Domain.Interfaces;
using rpa_data_collector.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<ICollectRepository, CollectRepository>();
builder.Services.AddScoped<ICollectService, CollectService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ??
                                   throw new InvalidOperationException("The key 'Jwt:Secret' was not found.")))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new
        OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Informe: Bearer {token}",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
    c.AddSecurityRequirement(doc => new
        OpenApiSecurityRequirement
        {
            {
                new
                    OpenApiSecuritySchemeReference("Bearer", doc),
                []
            }
        });
});


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limeter =>
    {
        limeter.PermitLimit = 10;
        limeter.Window = TimeSpan.FromSeconds(10);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        cors =>
        {
            cors.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    db.Database.CanConnect();
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();