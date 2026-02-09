namespace BattleshipWeb.DTOs.Responses
{
    public class GameStateResponse
    {
        public bool IsGameStarted { get; set; }
        public bool IsGameOver { get; set; }
        public string? WinnerName { get; set; }
        public string GamePhase { get; set; } = string.Empty;
        public string CurrentPlayerName { get; set; } = string.Empty;
        public List<PlayerResponse> Players { get; set; } = new();
    }
}
