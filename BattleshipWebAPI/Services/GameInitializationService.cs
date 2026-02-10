namespace BattleshipWeb.Services
{
    using BattleshipWeb.Interface;
    using BattleshipWeb.Models;

    using Microsoft.Extensions.Logging;

    public static class GameInitializationService
    {
        public static GameService CreateGame(List<string> playerNames, ILogger<GameService> logger)
        {
            var players = playerNames.Select(name => new Player(name) as IPlayer).ToList();
            return new GameService(players, logger);
        }
    }
}
