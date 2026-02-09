namespace BattleshipWeb.DTOs.Requests
{
    public class FireShotRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
