namespace BattleshipWeb.DTOs.Responses
{
    public class PlayerResponse
    {
        public string Name { get; set; } = string.Empty;
        public int ShipsPlaced { get; set; }
        public int ShipsRemaining { get; set; }
        public int ShipsDestroyed { get; set; }
        public bool IsReady { get; set; }
        public BoardResponse? Board { get; set; }
    }
}
