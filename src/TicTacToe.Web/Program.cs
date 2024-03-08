using TicTacToe.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHub<TicTacToeGameHub>("hub/game");

app.Run();