using BattleshipWeb.Models;
using Microsoft.Extensions.Logging;

namespace BattleshipWeb.Services
{
    public class GameSessionService
    {
        public GameService? CurrentGame { get; private set; }
        private readonly ILogger<GameService> _logger;

        public GameSessionService(ILogger<GameService> logger)
        {
            _logger = logger;
        }

        public void CreateGame(List<string> playerNames)
        {
            CurrentGame = GameInitializationService.CreateGame(playerNames, _logger);
        }

        public void ResetGame()
        {
            CurrentGame = null;
        }

        public bool HasActiveGame => CurrentGame != null;
    }
}
