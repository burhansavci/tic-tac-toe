namespace tic_tac_toe.Web.Domain;

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