using System.Security.Cryptography;
using jwt_sample.Api;
using jwt_sample.Api.Interface;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers();

ConfigurationManager configuration = builder.Configuration;
var issuret = configuration["Issurer"] ?? string.Empty;  //發行人
var audience = configuration["Audience"] ?? string.Empty; //受眾人
var publicKey = configuration["PublicKey"] ?? string.Empty;   //RSA 1024 公鑰
var privateKey = configuration["PrivateKey"] ?? string.Empty;   //RSA 1024 私鑰

var rsa = RSA.Create();
rsa.FromXmlString(publicKey ?? string.Empty);  // for rsa xml 
var key = new RsaSecurityKey(rsa);

//配置JWT認證服務
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //是否驗證發行人
        ValidateIssuer = true,
        ValidIssuer = issuret,//發行人
                              //是否驗證受眾人
        ValidateAudience = true,
        ValidAudience = audience,//受眾人
                                 //是否驗證金鑰
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true, //驗證生命週期
        RequireExpirationTime = true, //過期時間
        ClockSkew = TimeSpan.Zero
    };
});

//注入IToken
builder.Services.AddSingleton<IToken>(new Token(issuret, audience, privateKey, 30));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseRouting();

app.UseCors();

//1.先開啟認證
app.UseAuthentication();
//2.再開啟授權
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


