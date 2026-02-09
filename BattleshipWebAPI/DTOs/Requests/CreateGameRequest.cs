namespace BattleshipWeb.DTOs.Requests
{
    public class CreateGameRequest
    {
        public List<string> PlayerNames { get; set; } = new();
    }
}
