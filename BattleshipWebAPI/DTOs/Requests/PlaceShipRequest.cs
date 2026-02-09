namespace BattleshipWeb.DTOs.Requests
{
    public class PlaceShipRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public string ShipType { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Col { get; set; }
        public string Orientation { get; set; } = "Horizontal";
    }
}
