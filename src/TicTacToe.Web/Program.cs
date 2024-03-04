using TicTacToe.Web.Domain;

var builder = WebApplication.CreateBuilder(args);

var playerX = new Player.X("Player X");
var playerO = new Player.O("Player O");
var game = new Game(playerX, playerO);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/state", () =>
    {
        var board = game.Board;
        
        var visualizedBoard = string.Join(" --- ", [
            $"{board[0]?.Symbol ?? '0'} | {board[1]?.Symbol ?? '1'} | {board[2]?.Symbol ?? '2'}",
            $"{board[3]?.Symbol ?? '3'} | {board[4]?.Symbol ?? '4'} | {board[5]?.Symbol ?? '5'}",
            $"{board[6]?.Symbol ?? '6'} | {board[7]?.Symbol ?? '7'} | {board[8]?.Symbol ?? '8'}"
        ]);

        var state = new
        {
            visualizedBoard,
            game.Turn,
            game.Winner,
            game.IsOver,
            game.IsDraw
        };
        return TypedResults.Ok(state);
    })
    .WithName("State")
    .WithOpenApi();

app.MapPost("/play", (int position) =>
    {
        game.PlayTurn(position);
        return TypedResults.Ok("Move played");
    })
    .WithName("Play")
    .WithOpenApi();

app.MapPost("/reset", () =>
{
    game.Reset();
    return TypedResults.Ok("Game reset");
});

app.Run();