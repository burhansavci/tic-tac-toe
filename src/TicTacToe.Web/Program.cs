using TicTacToe.Web;
using TicTacToe.Web.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

builder.Services.AddSingleton<Game>();

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<TicTacToeGameHub>("hub/game");

app.Run();