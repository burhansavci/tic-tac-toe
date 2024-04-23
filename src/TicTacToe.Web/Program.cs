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

var games = new List<Game>
{
    new(),
    new(),
    new()
};

builder.Services.AddSingleton(games);

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<TicTacToeGameHub>("hub/game");

app.Run();