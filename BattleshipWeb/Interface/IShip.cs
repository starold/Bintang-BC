using BattleshipWeb.Enums;
namespace BattleshipWeb.Interface
{
    public interface IShip
    {
        ShipType ShipType {get;}
        int Size {get;}
        int HitCount{get; set;}
    }
}