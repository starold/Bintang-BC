using BattleshipWeb.Enums;
using BattleshipWeb.Interface;

namespace BattleshipWeb.Models
{
    public class Ship : IShip
    {
        public ShipType ShipType { get; private set; }
        public int Size { get; private set; }
        public int HitCount { get; set; }

        public Ship(ShipType shipType)
        {
            ShipType = shipType;
            HitCount = 0;
            switch (shipType)
            {
                case ShipType.Carrier:
                    Size = 5;
                    break;
                case ShipType.Battleship:
                    Size = 4;
                    break;
                case ShipType.Cruiser:
                    Size = 3;
                    break;
                default:
                    Size = 0;
                    break;
            }
        }
    }
}
