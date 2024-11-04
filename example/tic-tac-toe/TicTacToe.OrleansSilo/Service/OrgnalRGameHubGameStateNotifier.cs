using OrleansR.Core.Provider;
using TicTacToe.Interfaces.Hubs;
using TicTacToe.Interfaces.HubClients;

namespace TicTacToe.OrleansSilo.Service;

public class OrgnalRGameHubGameStateNotifier : IGameStateNotifier
{
    private readonly IHubContextProvider hubContextProvider;

    public OrgnalRGameHubGameStateNotifier(IHubContextProvider hubContextProvider)
    {
        this.hubContextProvider = hubContextProvider;
    }

    public void NotifyNewGameStateAvailable(string gameId)
    {
        var clientsInGroup = hubContextProvider.GetHubContext< IGameHub, IGameHubClient>()
            .Groups
            .GetGroup(gameId);

        // Ignore result
        _ = clientsInGroup.NewGameStateAvailable(gameId);
    }
}
