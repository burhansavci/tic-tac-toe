namespace TicTacToe.Web.Domain;

public record Board(int Size)
{
    private readonly Move?[] _moves = new Move?[Size];
    public IReadOnlyCollection<Move?> Moves => _moves;

    public Move? this[int position] => _moves[position];

    public void AddMove(Move move)
    {
        if (move.Position < 0 || move.Position >= Size)
            throw new Exception("Invalid position.");

        if (_moves[move.Position] != null)
            throw new Exception("Position is already taken.");

        _moves[move.Position] = move;
    }

    public void Reset()
    {
        for (var i = 0; i < Size; i++)
        {
            _moves[i] = null;
        }
    }
}