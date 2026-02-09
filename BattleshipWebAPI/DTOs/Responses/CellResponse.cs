namespace BattleshipWeb.DTOs.Responses
{
    public class CellResponse
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsShot { get; set; }
        public bool HasShip { get; set; }
        public string? ShipType { get; set; }
    }
}
