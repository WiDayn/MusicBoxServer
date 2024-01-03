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

// ��� JWT ��֤����
var key = Encoding.ASCII.GetBytes("�������һ���㹻���Ҹ��ӵ���Կ������16���ַ������ǻ�Ҫ�������һ�仰�Ź�16���ַ�"); //��Կ
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
// �Զ����м��
app.UseMiddleware<JwtMiddleware>();


app.UseAuthorization();

app.MapControllers();

app.Run();
