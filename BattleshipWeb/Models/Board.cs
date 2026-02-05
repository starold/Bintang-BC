using BattleshipWeb.Enums;
using BattleshipWeb.Interface;

namespace BattleshipWeb.Models
{
    public class Board : IBoard
    {
        public int Row {get; private set;}
        public int Col {get; private set;}
        public Cell[,] Cells {get; private set;}

        public Board(int rows, int columns)
        {
            Row = rows;
            Col = columns;
            Cells = new Cell[rows, columns];

            for (int r=0; r<rows; r++)
            {
                for(int c=0; c < columns; c++)
                {
                    Cells[r,c] = new Cell(new Position(r,c));

                }
            }
        }
    }
}
