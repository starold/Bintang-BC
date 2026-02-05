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

        public override string ToString()
        {
            return $"{(char)('A' + Row)}{Col}";
        }

        public static Position? Parse(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 2) return null;
            
            input = input.ToUpper();
            char rowChar = input[0];
            if (rowChar < 'A' || rowChar > 'J') return null; // Assuming 10 rows
            
            string colStr = input.Substring(1);
            if (int.TryParse(colStr, out int col))
            {
                 if (col >= 0 && col < 10)
                 {
                     return new Position(rowChar - 'A', col);
                 }
            }
            return null;
        }
    }
}