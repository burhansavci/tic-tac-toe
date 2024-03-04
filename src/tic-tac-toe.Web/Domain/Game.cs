namespace tic_tac_toe.Web.Domain;

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
    private Player _nextPlayer;
    private readonly Player.X _playerX;
    private readonly Player.O _playerO;
    
    public Game(Player.X playerX, Player.O playerO)
    {
        _playerX = playerX;
        _playerO = playerO;
        _nextPlayer = Random.Shared.Next(0, 2) == 0 ? playerX : playerO;
    }

    public Board Board { get; } = new(MaxTurn);
    public int Turn { get; private set; } = 1;
    public Player? Winner { get; private set; }
    public bool IsOver { get; private set; }
    public bool IsDraw => IsOver && Winner is null;

    public void PlayTurn(int position)
    {
        if (IsOver)
            throw new Exception("Game is over.");

        Board.AddMove(_nextPlayer.Move(position));
        Winner = CalculateWinner();
        IsOver = Winner is not null || Turn == MaxTurn;
        Turn++;
        _nextPlayer = _nextPlayer == _playerX ? _playerO : _playerX;
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
                return first.Symbol == _playerX.Symbol ? _playerX : _playerO;
            }
        }

        return null;
    }

    public void Reset()
    {
        Board.Reset();
        Turn = 1;
        Winner = null;
        IsOver = false;
        _nextPlayer = _nextPlayer == _playerX ? _playerO : _playerX;
    }
}