using BattleshipWeb.Enums;
using BattleshipWeb.Models;
namespace BattleshipWeb.Interface
{
    public interface IBoard
    {
        int Row{get;}
        int Col{get;}
        Cell[,] Cells{get;}
    }
    
}