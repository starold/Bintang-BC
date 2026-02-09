using System;
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
            Size = (int)Enum.Parse(typeof(ShipSize), shipType.ToString());

        }
    }
}
