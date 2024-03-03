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
        var result = new
        {
            FirstRow = new { First = board[0], Second = board[1], Third = board[2] },
            SecondRow = new { First = board[3], Second = board[4], Third = board[5] },
            ThirdRow = new { First = board[6], Second = board[7], Third = board[8] },
        };
        return TypedResults.Ok(result);
    })
    .WithName("State")
    .WithOpenApi();

app.MapPost("/play", (int position) =>
    {
        game.Play(position);
        if (!game.IsOver)
            return TypedResults.Ok("Game continues");

        var result = game.IsDraw ? "Draw" : $"{game.Winner!.Name} wins";
        return TypedResults.Ok(result);
    })
    .WithName("Play")
    .WithOpenApi();

app.Run();


public abstract record Player(string Name, char Symbol)
{
    public abstract Move Move(int position);

    public sealed record X(string Name) : Player(Name, 'x')
    {
        public override Move Move(int position)
        {
            return new Move.X(position);
        }
    }

    public sealed record O(string Name) : Player(Name, 'o')
    {
        public override Move Move(int position)
        {
            return new Move.O(position);
        }
    }
}

public abstract record Move(int Position, char Symbol)
{
    public sealed record X(int Position) : Move(Position, 'x');

    public sealed record O(int Position) : Move(Position, 'o');
}


public class Game
{
    private const int MaxTurn = 9;
    private Player _next;
    public Player.X PlayerX { get; }
    public Player.O PlayerO { get; }

    public Game(Player.X playerX, Player.O playerO)
    {
        PlayerX = playerX;
        PlayerO = playerO;
        _next = playerX;
    }

    public Game(Player.O playerO, Player.X playerX)
    {
        PlayerO = playerO;
        PlayerX = playerX;
        _next = playerO;
    }

    public Move?[] Board { get; } = new Move?[MaxTurn];
    public int Turn { get; private set; } = 1;
    public Player? Winner { get; private set; }
    public bool IsOver { get; private set; }
    public bool IsDraw => IsOver && Winner is null;

    public static int[][] WinningCombinations { get; set; } =
    [
        [0, 1, 2],
        [3, 4, 5],
        [6, 7, 8],
        [0, 3, 6],
        [1, 4, 7],
        [2, 5, 8],
        [0, 4, 8],
        [2, 4, 6]
    ];

    public void Play(int position)
    {
        if (IsOver)
            throw new Exception("Game is over.");

        var move = _next.Move(position);

        if (move.Position < 0 || move.Position >= Board.Length)
            throw new Exception("");

        if (Board[move.Position] != null)
            throw new Exception("");

        Board[move.Position] = move;
        Turn++;
        Winner = CalculateWinner();
        if (Winner is not null || Turn > MaxTurn)
        {
            IsOver = true;
            return;
        }

        _next = _next == PlayerX ? PlayerO : PlayerX;
    }

    private Player? CalculateWinner()
    {
        if (Turn < 4)
        {
            return null;
        }

        foreach (var combination in WinningCombinations)
        {
            var first = Board[combination[0]];
            var second = Board[combination[1]];
            var third = Board[combination[2]];

            if (first is null || second is null || third is null)
            {
                continue;
            }

            if (first.Symbol == second.Symbol && second.Symbol == third.Symbol)
            {
                return first.Symbol == PlayerX.Symbol ? PlayerX : PlayerO;
            }
        }

        return null;
    }

    public void Reset()
    {
        // ...
    }
}