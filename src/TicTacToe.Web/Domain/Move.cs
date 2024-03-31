namespace TicTacToe.Web.Domain;

public abstract record Move(int Position, char Symbol)
{
    public sealed record X(int Position) : Move(Position, 'X');

    public sealed record O(int Position) : Move(Position, 'O');
}