using BattleshipWeb.Enums;
using BattleshipWeb.Interface;
using BattleshipWeb.Models;


namespace BattleshipWeb
{
    class Program
    {
        static void Main(string[] args)

        {
            System.Console.Clear();
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
                Console.WriteLine($"{player.Name} fired at {target}: {result}!");
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

                game.PlaceShips(player);
                
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
                Game.RenderBoard(game.GetBoard(opponent), showShips: false);
                
                Console.WriteLine("\n--- YOUR BOARD ---");
                Game.RenderBoard(game.GetBoard(currentPlayer), showShips: true);

                Console.WriteLine("\nEnter coordinates to fire (e.g. A5), or 'exit' to quit:");
                var input = Console.ReadLine();
                
                if (input?.ToLower() == "exit") return;

                var pos = Position.Parse(input ?? "");
                
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
    }
}
