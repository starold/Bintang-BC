using BattleshipWeb.Enums;
using BattleshipWeb.Interface;

namespace BattleshipWeb.Models
{
    public class Player : IPlayer
    {
        public string Name {get; private set;}
        public Player(string name)
        {
            Name=name;
        }
    }
}