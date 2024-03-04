namespace tic_tac_toe.Web.Domain;

public abstract record Move(int Position, char Symbol)
{
    public sealed record X(int Position) : Move(Position, 'x');

    public sealed record O(int Position) : Move(Position, 'o');
}