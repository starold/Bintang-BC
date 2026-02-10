namespace BattleshipWeb.Services
{
    using BattleshipWeb.Enums;
    using BattleshipWeb.Interface;
    using BattleshipWeb.Models;
    using BattleshipWeb.Common;
    using BattleshipWeb.DTOs.Responses;
    using Microsoft.Extensions.Logging;

    public class GameService
    {
        private readonly ILogger<GameService> _logger;

        private List<IPlayer> _players;
        private Dictionary<IPlayer, IBoard> _boards;
        private Dictionary<IPlayer, List<IShip>> _playerShips;
        public IPlayer CurrentPlayer { get; private set; }
        public GameState State { get; private set; }
        public IPlayer? Winner { get; private set; }
        
        
        private const int BOARD_SIZE = 10;
        private const int SHIPS_PER_PLAYER = 3; // Carrier, Battleship, Cruiser

        public GameService(List<IPlayer> players, ILogger<GameService> logger) 
        {
            _logger = logger;
            _players = players;
            _boards = new Dictionary<IPlayer, IBoard>();
            _playerShips = new Dictionary<IPlayer, List<IShip>>();
            State = GameState.Setup;
            Winner = null;

            foreach (var player in _players)
            {
                _boards[player] = new Board(BOARD_SIZE, BOARD_SIZE);
                _playerShips[player] = new List<IShip>();
            }

            CurrentPlayer = _players.FirstOrDefault()!;

            _logger.LogInformation(
                "Game diinisialisasi dan dimuladi dari {@Players}",
                _players.Select(p => p.Name)
            );
        }

        public ServiceResult<IPlayer> ValidatePlayerTurn(string playerName)
        {
            var player = _players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                _logger.LogWarning(
                    "Validasi giliran salah. Giliran {Player}",
                    playerName
                );
                return ServiceResult<IPlayer>.Fail(
                    new ServiceError(ErrorType.Validation, $"Player '{playerName}' not found.")
                );
            }

            if (CurrentPlayer != player)
            {
                _logger.LogWarning(
                    "Bukan giliran {Player}, sekarang giliran {Current}",
                    playerName,
                    CurrentPlayer.Name
                );
                return ServiceResult<IPlayer>.Fail(
                    new ServiceError(ErrorType.Validation, $"It's not {playerName}'s turn. Current player is {CurrentPlayer.Name}.")
                );
            }

            return ServiceResult<IPlayer>.Success(player);
        }

        public IPlayer? GetPlayer(string playerName)
        {
            return _players.FirstOrDefault(p => p.Name == playerName);
        }

        public ServiceResult<bool> PlaceShip(string playerName, ShipType shipType, Position position, Orientation orientation)
        {
            if (State != GameState.Setup)
            {
                _logger.LogWarning(
                    "Penempatan kapal dilakukan diluar fase oleh {Player}",
                    playerName
                );
                return ServiceResult<bool>.Fail(
                    new ServiceError(ErrorType.Validation, "Cannot place ships outside of Setup phase.")
                );
            }

            var player = GetPlayer(playerName);
            if (player == null)
            {
                _logger.LogWarning(
                    "{Player} tidak ada",
                    playerName
                );
                return ServiceResult<bool>.Fail(
                    new ServiceError(ErrorType.NotFound, $"Player '{playerName}' not found.")
                );
            }

            var board = _boards[player];
            var ship = new Ship(shipType);

            // Check if this ship type is already placed
            if (_playerShips[player].Any(s => s.ShipType == shipType))
            {
                _logger.LogWarning(
                    "{Player} mencoba duplikasi kapal {ShipType}",
                    playerName, shipType
                );
                return ServiceResult<bool>.Fail(
                    new ServiceError(ErrorType.Validation, $"Ship type {shipType} is already placed.")
                );
            }

            // Validate placement
            if (!IsValidShipPlacement(board, ship, position, orientation))
            {
                _logger.LogWarning(
                    "{Player} menaruh kapal ditempat yang salah untuk kapal {ShipType} at {@Position}",
                    playerName, shipType, position
                );
                return ServiceResult<bool>.Fail(
                    new ServiceError(ErrorType.Validation, "Invalid ship placement. Out of bounds or overlapping with another ship.")
                );
            }
            // Place the ship
            int r = position.Row;
            int c = position.Col;

            for (int i = 0; i < ship.Size; i++)
            {
                if (orientation == Orientation.Horizontal)
                    board.Cells[r, c + i].Ship = ship;
                else
                    board.Cells[r + i, c].Ship = ship;
            }
            
            _playerShips[player].Add(ship);

            _logger.LogInformation(
                "{Player} telah menaruh kapal {ShipType} di posisi {@Position} dan {Orientation} dengan benar.",
                playerName, shipType, position, orientation
            );

            return ServiceResult<bool>.Success(true);
        }

        private bool IsValidShipPlacement(IBoard board, IShip ship, Position position, Orientation orientation)
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

        public ServiceResult<bool> StartBattle()
        {
            // Check if all players have placed all ships
            foreach (var player in _players)
            {
                if (_playerShips[player].Count < SHIPS_PER_PLAYER)
                {
                    _logger.LogWarning(
                        "Peperangan gagal.{Player} gagal menaruh kapal",
                        player.Name
                    );
                    return ServiceResult<bool>.Fail(
                        new ServiceError(ErrorType.Validation, $"Player {player.Name} has not placed all ships yet.")
                    );
                }
            }

            State = GameState.Battle;
            CurrentPlayer = _players[0];
            return ServiceResult<bool>.Success(true);
        }

        public ServiceResult<FireShotResponse> FireShot(string playerName, Position targetPosition)
        {
            if (State != GameState.Battle)
            {
                _logger.LogWarning(
                    "Tembakan ditaruh diluar battlephase oleh {Player}",
                    playerName
                );
                return ServiceResult<FireShotResponse>.Fail(
                    new ServiceError(ErrorType.Validation, "Cannot fire shots outside of Battle phase.")
                );
            }

            var validateResult = ValidatePlayerTurn(playerName);
            if (!validateResult.IsSuccess)
                return ServiceResult<FireShotResponse>.Fail(validateResult.Error!);

            var player = validateResult.Data!;
            var opponent = _players.First(p => p != player);
            var board = _boards[opponent];

            // Validate position
            if (targetPosition.Row < 0 || targetPosition.Row >= board.Row ||
                targetPosition.Col < 0 || targetPosition.Col >= board.Col)
                return ServiceResult<FireShotResponse>.Fail(
                    new ServiceError(ErrorType.Validation, "Invalid target position.")
                );

            var cell = board.Cells[targetPosition.Row, targetPosition.Col];

            // Check if already shot
            if (cell.IsShot)
            {
                _logger.LogWarning(
                    "(Player) mencoba menembak cell (@Target), tapi sudah ditembak."
                );
                return ServiceResult<FireShotResponse>.Fail(
                    new ServiceError(ErrorType.Validation, "This position has already been shot.")
                );
            }

            cell.IsShot = true;

            var response = new FireShotResponse
            {
                Row = targetPosition.Row,
                Col = targetPosition.Col,
                Result = "Miss",
                IsGameOver = false
            };

            if (cell.Ship != null)
            {
                cell.Ship.HitCount++;
                
                if (IsShipSunk(cell.Ship))
                {
                    response.Result = "Sink";
                    response.SunkShipType = cell.Ship.ShipType.ToString();
                }
                else
                {
                    response.Result = "Hit";
                }
                _logger.LogInformation(
                    "{Player} telah menembak {@Target} dengan hasil {Result}",
                    playerName,
                    targetPosition,
                    response.Result
                );
            }

            // Check for winner
            var winner = CheckWinner();
            if (winner != null)
            {
                State = GameState.Finished;
                Winner = winner;
                response.IsGameOver = true;
                response.WinnerName = winner.Name;

                _logger.LogInformation(
                    "Game selesai. Pemenangnya adalah {Winner}",
                    Winner.Name
                );
            }

            return ServiceResult<FireShotResponse>.Success(response);
        }

        public void SwitchTurn()
        {
            CurrentPlayer = _players.First(p => p != CurrentPlayer);
            _logger.LogInformation(
                "Ganti giliran. Sekarang giliran {Player}",
                CurrentPlayer.Name
            );
        }

        private bool IsShipSunk(IShip ship)
        {
            return ship.HitCount >= ship.Size;
        }

        private IPlayer? CheckWinner()
        {
            var opponent = _players.First(p => p != CurrentPlayer);
            var opponentShips = _playerShips[opponent];

            if (opponentShips.All(s => IsShipSunk(s)))
            {
                return CurrentPlayer;
            }

            return null;
        }

        public GameStateResponse GetGameState()
        {
            var response = new GameStateResponse
            {
                IsGameStarted = true,
                IsGameOver = State == GameState.Finished,
                WinnerName = Winner?.Name,
                GamePhase = State.ToString(),
                CurrentPlayerName = CurrentPlayer.Name,
                Players = new List<PlayerResponse>()
            };

            foreach (var player in _players)
            {
                var ships = _playerShips[player];
                var playerResponse = new PlayerResponse
                {
                    Name = player.Name,
                    ShipsPlaced = ships.Count,
                    ShipsRemaining = ships.Count(s => !IsShipSunk(s)),
                    ShipsDestroyed = ships.Count(s => IsShipSunk(s)),
                    IsReady = ships.Count >= SHIPS_PER_PLAYER,
                    Board = GetBoardResponse(player)
                };
                response.Players.Add(playerResponse);
            }

            return response;
        }

        public BoardResponse GetBoardResponse(IPlayer player, bool hideShips = false)
        {
            var board = _boards[player];
            var response = new BoardResponse
            {
                Rows = board.Row,
                Cols = board.Col,
                Cells = new List<List<CellResponse>>()
            };

            for (int r = 0; r < board.Row; r++)
            {
                var row = new List<CellResponse>();
                for (int c = 0; c < board.Col; c++)
                {
                    var cell = board.Cells[r, c];
                    var cellResponse = new CellResponse
                    {
                        Row = r,
                        Col = c,
                        IsShot = cell.IsShot,
                        HasShip = hideShips ? (cell.IsShot && cell.Ship != null) : (cell.Ship != null),
                        ShipType = (hideShips && !cell.IsShot) ? null : cell.Ship?.ShipType.ToString()
                    };
                    row.Add(cellResponse);
                }
                response.Cells.Add(row);
            }

            return response;
        }

        public int GetShipsPlaced(IPlayer player)
        {
            return _playerShips[player].Count;
        }

        public bool IsPlayerReady(IPlayer player)
        {
            return _playerShips[player].Count >= SHIPS_PER_PLAYER;
        }

        public bool AreAllPlayersReady()
        {
            return _players.All(p => IsPlayerReady(p));
        }
    }
}
