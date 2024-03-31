namespace TicTacToe.Web.Domain;

public class Game
{
    private const int MaxTurn = 9;

    private static readonly int[][] WinningCombinations =
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

    public string Id { get; } = Guid.NewGuid().ToString();
    public Player.X? PlayerX { get; private set; }
    public Player? NextPlayer { get; private set; }
    public Player.O? PlayerO { get; private set; }
    public Board Board { get; } = new(MaxTurn);
    public int Turn { get; private set; } = 1;
    public Player? Winner { get; private set; }
    public bool IsDraw => State == GameState.Over && Winner is null;
    public bool IsFull => PlayerX is not null && PlayerO is not null;
    public bool CanJoin => !IsFull && State == GameState.WaitingForPlayers;
    public GameState State { get; private set; } = GameState.WaitingForPlayers;

    public void PlayTurn(int position)
    {
        if (State != GameState.Started)
            throw new Exception("Game is not started.");

        Board.AddMove(NextPlayer!.Move(position));

        Winner = CalculateWinner();
        if (Winner is not null || Turn == MaxTurn)
            State = GameState.Over;

        Turn++;
        NextPlayer = NextPlayer == PlayerX ? PlayerO : PlayerX;
    }

    public void Reset()
    {
        Board.Reset();
        Turn = 1;
        Winner = null;
        State = GameState.Started;
        NextPlayer = NextPlayer == PlayerX ? PlayerO : PlayerX;
    }

    public void Start()
    {
        if (PlayerX is null || PlayerO is null)
            throw new Exception("Game is not full.");

        if (State == GameState.Started)
            throw new Exception("Game is already started.");

        if (State == GameState.Over)
            throw new Exception("Game is already over.");

        State = GameState.Started;
        NextPlayer = Random.Shared.Next(0, 2) == 0 ? PlayerX : PlayerO;
    }

    public Player Join(string playerName)
    {
        if (PlayerX is null)
        {
            PlayerX = new Player.X(playerName);
            return PlayerX;
        }

        PlayerO = new Player.O(playerName);
        return PlayerO;
    }

    public void Leave(string playerName)
    {
        if (PlayerX?.Name == playerName)
        {
            PlayerX = null;
            State = GameState.WaitingForPlayers;
        }
        else if (PlayerO?.Name == playerName)
        {
            PlayerO = null;
            State = GameState.WaitingForPlayers;
        }
    }

    private Player? CalculateWinner()
    {
        if (Turn < 4)
            return null;

        foreach (var combination in WinningCombinations)
        {
            var first = Board[combination[0]];
            var second = Board[combination[1]];
            var third = Board[combination[2]];

            if (first is null || second is null || third is null)
                continue;

            if (first.Symbol == second.Symbol && second.Symbol == third.Symbol)
                return first.Symbol == PlayerX?.Symbol ? PlayerX : PlayerO;
        }

        return null;
    }
}