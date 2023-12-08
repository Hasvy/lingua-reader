using BlazorServer;
using BlazorServer.Migrations.DictionaryDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Objects.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7215")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDbContext<DictionaryDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Dictionary"))
//);

//Authentication
builder.Services.AddDefaultIdentity<AppUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<AppUser, AppDbContext>();   // options =>
//    {
//        options.IdentityResources["openid"].UserClaims.Add("name");
//        options.ApiResources.Single().UserClaims.Add("name");
//        options.IdentityResources["openid"].UserClaims.Add("role");
//        options.ApiResources.Single().UserClaims.Add("role");
//    });
//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddTransient<IProfileService, ProfileService>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

builder.Services.Configure<IdentityOptions>(options =>
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
//app.UseCors(policy =>
//    policy.WithOrigins("http://localhost:5284", "https://localhost:7215")
//    .AllowAnyMethod()
//    .WithHeaders(HeaderNames.ContentType)
//);

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();
