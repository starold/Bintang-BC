using BattleshipWeb.Enums;
using BattleshipWeb.Interface;
namespace BattleshipWeb.Models
{
    public class Position
    {
        public int Row{get; set;}
        public int Col{get; set;}
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public override bool Equals(object? obj)
        {
            if(obj is Position p)
            {
                return Row == p.Row && Col == p.Col;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return System.HashCode.Combine(Row, Col);
        }
    }
}