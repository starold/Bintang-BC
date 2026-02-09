namespace BattleshipWeb.DTOs.Responses
{
    public class ShipResponse
    {
        public string Type { get; set; } = string.Empty;
        public int Size { get; set; }
        public int HitCount { get; set; }
        public bool IsSunk { get; set; }
    }
}
