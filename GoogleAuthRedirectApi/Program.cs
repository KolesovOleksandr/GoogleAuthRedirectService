using GoogleAuthRedirectApi.Models;
using GoogleAuthRedirectApi.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<TokensDbSettings>(builder.Configuration.GetSection(nameof(TokensDbSettings)));
builder.Services.AddSingleton<ITokensDbSettings>(sp => sp.GetRequiredService<IOptions<TokensDbSettings>>().Value);
builder.Services.Configure<GoogleDriveSettings>(builder.Configuration.GetSection(nameof(GoogleDriveSettings)));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleDriveSettings>>().Value);
builder.Services.AddTransient<IUserTokensRepository, UserTokensRepository>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
