using BattleshipWeb.Enums;
using BattleshipWeb.Interface;
namespace BattleshipWeb.Models
{
    public class Cell : ICell
    {
        public Position Position{get; private set;}
        public IShip? Ship {get; set;}
        public bool IsShot{get; set;}
        public Cell(Position position)
        {
            Position = position;
            Ship = null;
            IsShot = false;
        }
    }
}