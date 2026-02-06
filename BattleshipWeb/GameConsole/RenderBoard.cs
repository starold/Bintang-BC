using System;
using BattleshipWeb.Interface;
using BattleshipWeb.Models;

namespace BattleshipWeb.GameConsole
{
    public class RenderBoard
    {
        public static void Render(IBoard board, bool showShips)
        {
            Console.Write("  ");
            for (int c = 0; c < board.Col; c++) Console.Write(c + " ");
            Console.WriteLine();

            for (int r = 0; r < board.Row; r++)
            {
                Console.Write((char)('A' + r) + " ");
                for (int c = 0; c < board.Col; c++)
                {
                    var cell = board.Cells[r, c];
                    char symbol = '~'; // Water
                    
                    if (cell.IsShot)
                    {
                        symbol = (cell.Ship != null) ? 'X' : 'O';
                    }
                    else
                    {
                        if (showShips && cell.Ship != null)
                        {
                            symbol = 'S'; // Ship
                        }
                    }
                    Console.Write(symbol + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
