namespace BattleshipWeb.DTOs.Responses
{
    public class FireShotResponse
    {
        public string Result { get; set; } = string.Empty; // Hit, Miss, Sink
        public int Row { get; set; }
        public int Col { get; set; }
        public string? SunkShipType { get; set; }
        public bool IsGameOver { get; set; }
        public string? WinnerName { get; set; }
    }
}
