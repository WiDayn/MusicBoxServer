using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MusicBoxServer.Dto;
using MusicBoxServer.Middleware;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddScoped<AlbumService>();
builder.Services.AddScoped<ArtistService>();
builder.Services.AddScoped<ConcertService>();
builder.Services.AddScoped<PlayListService>();
builder.Services.AddScoped<SongService>();
builder.Services.AddScoped<UserService>();

// 添加 JWT 认证服务
var key = Encoding.ASCII.GetBytes("这里放置一个足够长且复杂的密钥，至少16个字符，但是还要多加上这一句话才够16个字符"); //密钥
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 自定义中间件
app.UseMiddleware<JwtMiddleware>();


app.UseAuthorization();

app.MapControllers();

app.Run();
