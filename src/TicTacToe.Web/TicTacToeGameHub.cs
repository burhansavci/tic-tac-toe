using Microsoft.AspNetCore.SignalR;
using TicTacToe.Web.Domain;

namespace TicTacToe.Web;

public class TicTacToeGameHub : Hub<ITicTacToeGameHubClient>
{
    private readonly Game _game;

    public TicTacToeGameHub(Game game)
    {
        _game = game;
    }

    public async Task JoinGame()
    {
        var joinedGame = _game.TryJoin(Context.ConnectionId, out var player);

        if (!joinedGame) throw new Exception("You can't join this game.");

        await Groups.AddToGroupAsync(Context.ConnectionId, _game.Id);

        if (_game.State == GameState.Started) await Clients.Group(_game.Id).GameStateChange(new GameStateChangeResponse(_game.State, _game.Winner, _game.Board, _game.NextPlayer, player));
    }

    public async Task LeaveGame()
    {
        _game.Leave(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, _game.Id);
        
        await Clients.Group(_game.Id).GameStateChange(new GameStateChangeResponse(_game.State, _game.Winner, _game.Board, _game.NextPlayer,));
    }

    public async Task PlayTurn(int position)
    {
        if (Context.ConnectionId != _game.NextPlayer!.Name)
        {
            throw new Exception("It's not your turn.");
        }

        _game.PlayTurn(position);

        var response = new GameStateChangeResponse(_game.State, _game.Winner, _game.Board, _game.NextPlayer);
        await Clients.Group(_game.Id).GameStateChange(response);
    }

    public async Task ResetGame()
    {
        _game.Reset();
        await Clients.Group(_game.Id).GameStateChange(new GameStateChangeResponse(_game.State, _game.Winner, _game.Board, _game.NextPlayer));
    }
}

public interface ITicTacToeGameHubClient
{
    Task GameStateChange(GameStateChangeResponse response);
}

public record JoinGameResponse(Player? Player, bool CanJoin, GameState State);

public record GameStateChangeResponse(GameState State, Player? Winner, Board Board, Player? NextPlayer, Player CurrentPlayer);