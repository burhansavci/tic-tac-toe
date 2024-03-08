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
        if (_game.CanJoin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _game.Id);
            _game.Join(Context.ConnectionId);

            if (_game.IsFull)
            {
                _game.Start();
                await Clients.All.GameStarted();
            }
        }
        else
        {
            await Clients.All.GameFull();
        }
    }

    public async Task LeaveGame()
    {
        _game.Leave(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, _game.Id);
        await Clients.All.GameNotFull();
    }

    public async Task PlayTurn(int position)
    {
        if (Context.ConnectionId != _game.NextPlayer!.Name)
        {
            throw new Exception("It's not your turn.");
        }
        
        _game.PlayTurn(position);
        
        await Clients.All.GameStateChange();
    }

    public async Task ResetGame()
    {
        _game.Reset();
        await Clients.All.GameNotFull();
    }
}

public interface ITicTacToeGameHubClient
{
    Task GameFull();
    Task GameNotFull();
    Task GameStateChange();
    Task GameStarted();
}