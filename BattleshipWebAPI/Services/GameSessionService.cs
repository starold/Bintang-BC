using BattleshipWeb.Models;

namespace BattleshipWeb.Services
{
    public class GameSessionService
    {
        public GameService? CurrentGame { get; private set; }

        public void CreateGame(List<string> playerNames)
        {
            CurrentGame = GameInitializationService.CreateGame(playerNames);
        }

        public void ResetGame()
        {
            CurrentGame = null;
        }

        public bool HasActiveGame => CurrentGame != null;
    }
}
