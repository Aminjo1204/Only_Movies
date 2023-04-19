using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OnlyMoviesProject.Webapi.Infrastructure;
using System;
using Webapi;

var builder = WebApplication.CreateBuilder(args);
// JWT Authentication ******************************************************************************
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;

byte[] secret = Convert.FromBase64String(builder.Configuration["Secret"]);
builder.Services
    .AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
// *****

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(OnlyMoviesProject.Application.Dto.MappingProfile));
builder.Services.AddDbContext<OnlyMoviesContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
    });
}

var app = builder.Build();
// Other code
if (app.Environment.IsDevelopment())
{
    // We will create a fresh sql server container in development mode. For performance reasons,
    // you can disable deleteAfterShutdown because in development mode the database is deleted
    // before it is generated.
    try
    {
        await app.UseSqlServerContainer(
            containerName: "spengernews_sqlserver", version: "latest",
            connectionString: app.Configuration.GetConnectionString("Default"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }
}

// Other code
if (app.Environment.IsDevelopment())
{
    // We will create a fresh sql server container in development mode. For performance reasons,
    // you can disable deleteAfterShutdown because in development mode the database is deleted
    // before it is generated.
    try
    {
        await app.UseSqlServerContainer(
            containerName: "OnlyMovies_sqlserver", version: "latest",
            connectionString: app.Configuration.GetConnectionString("Default"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }
}


app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    using (var db = scope.ServiceProvider.GetRequiredService<OnlyMoviesContext>())
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Seed();
    }
    app.UseCors();
}


// Creating the database.
using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<OnlyMoviesContext>())
    {
        db.CreateDatabase(isDevelopment: app.Environment.IsDevelopment());
    }
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();