namespace BattleshipWeb.DTOs.Responses
{
    public class BoardResponse
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public List<List<CellResponse>> Cells { get; set; } = new();
    }
}
