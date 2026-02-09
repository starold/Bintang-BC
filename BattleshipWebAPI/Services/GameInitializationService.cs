namespace BattleshipWeb.Services
{
    using BattleshipWeb.Interface;
    using BattleshipWeb.Models;

    public static class GameInitializationService
    {
        public static GameService CreateGame(List<string> playerNames)
        {
            var players = playerNames.Select(name => new Player(name) as IPlayer).ToList();
            return new GameService(players);
        }
    }
}
