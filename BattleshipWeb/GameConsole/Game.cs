using BattleshipWeb.Enums;
using BattleshipWeb.Interface;
using BattleshipWeb.Models;
using BattleshipWeb.GameConsole;


namespace BattleshipWeb.Models
{
    public delegate void ShotFiredHandler(IPlayer player, Position target, ShotResult result);
    public delegate void GameFinishedHandler(IPlayer winner);

    public class Game
    {
        private List<IPlayer> _players;
        private Dictionary<IPlayer, IBoard> _board;
        public IPlayer CurrentPlayer { get; set; }
        public GameState State { get; set; }
        public List<Position> ShotHistory { get; private set; }

        public event ShotFiredHandler OnShotFired;
        public event GameFinishedHandler OnGameFinished;

        public Game(List<IPlayer> players, IBoard boardTemplate)
        {
            _players = players;
            _board = new Dictionary<IPlayer, IBoard>();
            ShotHistory = new List<Position>();
            State = GameState.Setup;

            // Initialize boards regarding dimensions
            foreach (var player in _players)
            {
                _board[player] = new Board(boardTemplate.Row, boardTemplate.Col);
            }

            CurrentPlayer = _players.FirstOrDefault();
        }

        public void Start()
        {
            State = GameState.Setup;
            CurrentPlayer = _players.FirstOrDefault();
            ShotHistory.Clear();
        }

        public bool PlaceShip(ShipType shipType, Position position, Orientation orientation)
        {
            if (State != GameState.Setup) return false;

            var board = _board[CurrentPlayer];
            var ship = new Ship(shipType);

            if (IsValidShipPlacement(board, ship, position, orientation))
            {
                int r = position.Row;
                int c = position.Col;

                for (int i = 0; i < ship.Size; i++)
                {
                    if (orientation == Orientation.Horizontal)
                        board.Cells[r, c + i].Ship = ship;
                    else
                        board.Cells[r + i, c].Ship = ship;
                }
                return true;
            }
            return false;
        }

        public bool IsValidShipPlacement(IBoard board, IShip ship, Position position, Orientation orientation)
        {
            int r = position.Row;
            int c = position.Col;

            if (r < 0 || c < 0) return false;

            if (orientation == Orientation.Horizontal)
            {
                if (c + ship.Size > board.Col) return false;
                for (int i = 0; i < ship.Size; i++)
                {
                    if (board.Cells[r, c + i].Ship != null) return false;
                }
            }
            else
            {
                if (r + ship.Size > board.Row) return false;
                for (int i = 0; i < ship.Size; i++)
                {
                    if (board.Cells[r + i, c].Ship != null) return false;
                }
            }
            return true;
        }

        public IPlayer GetCurrentPlayer()
        {
            return CurrentPlayer;
        }

        public IBoard GetBoard(IPlayer player)
        {
            return _board[player];
        }

        public ShotResult FireShot(Position targetPosition)
        {
            if (State != GameState.Battle) return ShotResult.Miss;

            var opponent = _players.First(p => p != CurrentPlayer);
            var board = _board[opponent];

            if (targetPosition.Row < 0 || targetPosition.Row >= board.Row || targetPosition.Col < 0 || targetPosition.Col >= board.Col)
                return ShotResult.Miss;

            var cell = board.Cells[targetPosition.Row, targetPosition.Col];
            
           

            cell.IsShot = true;

            ShotResult result = ShotResult.Miss;
            if (cell.Ship != null)
            {

            }
            
          
            cell = board.Cells[targetPosition.Row, targetPosition.Col];
            bool isNewShot = !cell.IsShot;
            cell.IsShot = true;

            if (cell.Ship != null)
            {
                if (isNewShot)
                {
                     cell.Ship.HitCount++;
                }
                result = IsShipFullyHit(cell.Ship) ? ShotResult.Sink : ShotResult.Hit;
            }

            ShotHistory.Add(targetPosition);
            
            OnShotFired?.Invoke(CurrentPlayer, targetPosition, result);

            var winner = CheckWinner();
            if (winner != null)
            {
                State = GameState.Finished;
                OnGameFinished?.Invoke(winner);
            }

            return result;
        }

        public bool IsPositionAlreadyShot(IBoard board, Position position)
        {
            if (position.Row < 0 || position.Row >= board.Row || position.Col < 0 || position.Col >= board.Col)
                return false;
            return board.Cells[position.Row, position.Col].IsShot;
        }

        public bool IsShipFullyHit(IShip ship)
        {
            return ship.HitCount >= ship.Size;
        }

        public void SwitchTurn()
        {
            CurrentPlayer = _players.First(p => p != CurrentPlayer);
        }

        public IPlayer? CheckWinner()
        {
            var opponent = _players.First(p => p != CurrentPlayer);
            var board = _board[opponent];

            var ships = new HashSet<IShip>();
            bool hasShips = false;
            foreach (var cell in board.Cells)
            {
                if (cell.Ship != null)
                {
                    ships.Add(cell.Ship);
                    hasShips = true;
                }
            }
            
            if (!hasShips) return null; // Logic check: if no ships placed, nobody wins or auto win?
            
            if (ships.All(s => IsShipFullyHit(s)))
            {
                return CurrentPlayer;
            }
            return null;
        }

        public int GetDestroyedShipCount(IPlayer player)
        {
            var board = _board[player];
            var ships = new HashSet<IShip>();
            
            foreach (var cell in board.Cells)
            {
                if (cell.Ship != null)
                {
                    ships.Add(cell.Ship);
                }
            }
            
            return ships.Count(s => IsShipFullyHit(s));
        }

        public void PlaceShips(IPlayer player)
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
                    RenderBoard.Render(GetBoard(player), showShips: true);
                    
                    Console.WriteLine("Enter start coordinate (e.g. A0):");
                    var input = Console.ReadLine();
                    var pos = Position.Parse(input ?? "");
                    if (pos == null) { Console.WriteLine("Invalid coord."); Console.ReadKey(); continue; }

                    Console.WriteLine("Enter orientation (H for Horizontal, V for Vertical):");
                    var orientInput = Console.ReadLine()?.ToUpper();
                    Orientation orient = (orientInput == "V" || orientInput == "VERTICAL") ? Orientation.Vertical : Orientation.Horizontal;

                    // Swap player if needed to ensure we place for the correct player
                    // In the original Program.cs, game.CurrentPlayer was set before calling PlaceShips.
                    // Here we ensure it matches the passed player.
                    var previousPlayer = CurrentPlayer;
                    CurrentPlayer = player;
                    if (PlaceShip(type, pos, orient))
                    {
                        placed = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid placement (Out of bounds or overlap). Try again.");
                        Console.ReadKey();
                    }
                    CurrentPlayer = previousPlayer;
                }
            }
        }


    }
}
