namespace MonopolyBackend.DTOs.Responses
{
    public record GameStateResponse
    {
        public bool IsGameStarted { get; init; }
        public bool IsGameOver { get; init; }
        public string? WinnerName { get; init; }
        public int CurrentTurn { get; init; }
        public string CurrentPlayerName { get; init; } = string.Empty;
        public List<string> AvailableActions { get; init; } = new();
        public List<PlayerResponse> Players { get; init; } = new();
        public List<PropertyResponse> AllProperties { get; init; } = new();
    }
}
