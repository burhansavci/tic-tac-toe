using Microsoft.AspNetCore.SignalR;
using TicTacToe.Web.Domain;

namespace TicTacToe.Web;

public class TicTacToeGameHub : Hub<ITicTacToeGameHubClient>
{
    private readonly List<Game> _games;
    private const string GamesStatutesGroup = "GamesStatutes";

    public TicTacToeGameHub(List<Game> games)
    {
        _games = games;
    }

    public async Task<List<GameStateChangeResponse>> SubscribeToGamesStatutes()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GamesStatutesGroup);
        
        return _games.Select(x => new GameStateChangeResponse(x.Id, x.State, x.Winner, x.Board, x.NextPlayer)).ToList();
    }

    public async Task UnsubscribeFromGamesStatutes() =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GamesStatutesGroup);

    public async Task<JoinGameResponse> JoinGame(string gameId)
    {
        var game = _games.Single(x => x.Id == gameId);

        var joinedGame = game.TryJoin(Context.ConnectionId, out var player);

        if (!joinedGame) throw new Exception("You can't join this game.");

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);

        if (game.State == GameState.Started)
        {
            var response = new GameStateChangeResponse(game.Id, game.State, game.Winner, game.Board, game.NextPlayer);
            await Clients.Group(game.Id).GameStateChange(response);
            await Clients.Group(GamesStatutesGroup).GameStateChange(response);
        }

        return new JoinGameResponse(game.State, player);
    }

    public async Task LeaveGame(string gameId)
    {
        var game = _games.Single(x => x.Id == gameId);

        game.Leave(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);

        var response = new GameStateChangeResponse(game.Id, game.State, game.Winner, game.Board, game.NextPlayer);
        await Clients.Group(game.Id).GameStateChange(response);
        await Clients.Group(GamesStatutesGroup).GameStateChange(response);
    }

    public async Task PlayTurn(string gameId, int position)
    {
        var game = _games.Single(x => x.Id == gameId);

        if (Context.ConnectionId != game.NextPlayer!.Name)
            throw new Exception("It's not your turn.");

        game.PlayTurn(position);

        var response = new GameStateChangeResponse(game.Id, game.State, game.Winner, game.Board, game.NextPlayer);
        await Clients.Group(game.Id).GameStateChange(response);
        await Clients.Group(GamesStatutesGroup).GameStateChange(response);
    }

    public async Task ResetGame(string gameId)
    {
        var game = _games.Single(x => x.Id == gameId);

        game.Reset();

        var response = new GameStateChangeResponse(game.Id, game.State, game.Winner, game.Board, game.NextPlayer);
        await Clients.Group(game.Id).GameStateChange(response);
        await Clients.Group(GamesStatutesGroup).GameStateChange(response);
    }
}

public interface ITicTacToeGameHubClient
{
    Task GameStateChange(GameStateChangeResponse response);
}

public record JoinGameResponse(GameState State, Player Player);

public record GameStateChangeResponse(string Id, GameState State, Player? Winner, Board Board, Player? NextPlayer);