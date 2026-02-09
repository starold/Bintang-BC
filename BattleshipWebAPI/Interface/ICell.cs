using BattleshipWeb.Enums;
using BattleshipWeb.Models;
namespace BattleshipWeb.Interface
{
    public interface ICell
    {
        Position Position {get;}
        IShip? Ship{get;}
        bool IsShot{get;}
    }
}