namespace TicTacToe.Web.Domain;

public abstract record Player(string Name, char Symbol, bool Next)
{
    public abstract Move Move(int position);

    public sealed record X(string Name) : Player(Name, 'X', false)
    {
        public override Move Move(int position)
        {
            return new Move.X(position);
        }
    }

    public sealed record O(string Name) : Player(Name, 'O', false)
    {
        public override Move Move(int position)
        {
            return new Move.O(position);
        }
    }
}