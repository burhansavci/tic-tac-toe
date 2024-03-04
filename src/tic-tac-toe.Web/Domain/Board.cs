namespace tic_tac_toe.Web.Domain;

public record Board(int Size)
{
    private Move?[] Moves { get; } = new Move?[Size];

    public Move? this[int position] => Moves[position];

    public void AddMove(Move move)
    {
        if (move.Position < 0 || move.Position >= Size)
            throw new Exception("Invalid position.");

        if (Moves[move.Position] != null)
            throw new Exception("Position is already taken.");

        Moves[move.Position] = move;
    }

    public void Reset()
    {
        for (var i = 0; i < Size; i++)
        {
            Moves[i] = null;
        }
    }
}