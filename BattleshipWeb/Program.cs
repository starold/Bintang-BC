using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipWeb.Enums;
using BattleshipWeb.Interface;
using BattleshipWeb.Models;


namespace BattleshipWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Console Battleship!");

            // 1. Setup Players
            Console.Write("Enter Player 1 Name: ");
            string p1Name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Player 1";
            
            Console.Write("Enter Player 2 Name: ");
            string p2Name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Player 2";

            var player1 = new Player(p1Name);
            var player2 = new Player(p2Name);
            var players = new List<IPlayer> { player1, player2 };

            // 2. Setup Game (10x10 Board)
            var boardTemplate = new Board(10, 10);
            var game = new Game(players, boardTemplate);

            game.OnShotFired += (player, target, result) =>
            {
                Console.WriteLine($"{player.Name} fired at {ToCoordinate(target)}: {result}!");
            };

            game.OnGameFinished += (winner) =>
            {
                Console.WriteLine($"\nGAME OVER! The winner is {winner.Name}!");
            };

            game.Start();

            // 3. Placement Phase
            foreach (var player in players)
            {
                game.CurrentPlayer = player;
                
                Console.Clear();
                Console.WriteLine($"--- {player.Name}'s Turn to Place Ships ---");
                Console.WriteLine("Press any key to start placement (hidden from opponent)...");
                Console.ReadKey();

                PlaceShips(game, player);
                
                Console.Clear();
                Console.WriteLine("Ships placed! Press any key to switch...");
                Console.ReadKey();
            }

            // 4. Battle Phase
            game.State = GameState.Battle;
            // Force CurrentPlayer back to P1 to start
            game.CurrentPlayer = players[0];
            
            while (game.State == GameState.Battle)
            {
                Console.Clear();
                var currentPlayer = game.GetCurrentPlayer();
                var opponent = players.FirstOrDefault(p => p != currentPlayer);
                
                Console.WriteLine($"--- {currentPlayer.Name}'s Turn ---");
                Console.WriteLine($"Opponent: {opponent.Name}");
                
                // Display Stats
                int mySunk = game.GetDestroyedShipCount(currentPlayer);
                int oppSunk = game.GetDestroyedShipCount(opponent);
                Console.WriteLine($"Stat: Your Ships Lost: {mySunk} | Opponent Ships Destroyed: {oppSunk}");
                Console.WriteLine();

                // Show Boards
                // We'll show them stacked for simplicity in console.
                Console.WriteLine("--- OPPONENT BOARD (Target) ---");
                RenderBoard(game.GetBoard(opponent), showShips: false);
                
                Console.WriteLine("\n--- YOUR BOARD ---");
                RenderBoard(game.GetBoard(currentPlayer), showShips: true);

                Console.WriteLine("\nEnter coordinates to fire (e.g. A5), or 'exit' to quit:");
                var input = Console.ReadLine();
                
                if (input?.ToLower() == "exit") return;

                var pos = ParseCoordinate(input);
                
                if (pos == null) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid coordinate. Format: A0-J9. Press any key.");
                    Console.ResetColor();
                    Console.ReadKey();
                    continue;
                }

                var opponentBoard = game.GetBoard(opponent);
                if (game.IsPositionAlreadyShot(opponentBoard, pos))
                {
                     Console.ForegroundColor = ConsoleColor.Yellow;
                     Console.WriteLine("You already shot there! Press any key.");
                     Console.ResetColor();
                     Console.ReadKey();
                     continue;
                }

                var result = game.FireShot(pos);
                Console.WriteLine("Press any key to end turn...");
                Console.ReadKey();
                
                if (game.State == GameState.Battle)
                {
                    game.SwitchTurn();
                }
            }
            
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }

        static void PlaceShips(Game game, IPlayer player)
        {
            // Placing one of each type for simplicity
            var types = new[] { ShipType.Carrier, ShipType.Battleship, ShipType.Cruiser };

            foreach (var type in types)
            {
                bool placed = false;
                while (!placed)
                {
                    Console.Clear();
                    Console.WriteLine($"{player.Name} Placing {type} (Size: {new Ship(type).Size})");
                    RenderBoard(game.GetBoard(player), showShips: true);
                    
                    Console.WriteLine("Enter start coordinate (e.g. A0):");
                    var input = Console.ReadLine();
                    var pos = ParseCoordinate(input);
                    if (pos == null) { Console.WriteLine("Invalid coord."); Console.ReadKey(); continue; }

                    Console.WriteLine("Enter orientation (H for Horizontal, V for Vertical):");
                    var orientInput = Console.ReadLine()?.ToUpper();
                    Orientation orient = (orientInput == "V" || orientInput == "VERTICAL") ? Orientation.Vertical : Orientation.Horizontal;

                    if (game.PlaceShip(type, pos, orient))
                    {
                        placed = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid placement (Out of bounds or overlap). Try again.");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void RenderBoard(IBoard board, bool showShips)
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

        static Position ParseCoordinate(string input)
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

        static string ToCoordinate(Position p)
        {
            return $"{(char)('A' + p.Row)}{p.Col}";
        }
    }
}
