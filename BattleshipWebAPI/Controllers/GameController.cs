using Microsoft.AspNetCore.Mvc;
using BattleshipWeb.Services;
using BattleshipWeb.DTOs.Requests;
using BattleshipWeb.DTOs.Responses;
using BattleshipWeb.Enums;
using BattleshipWeb.Models;

namespace BattleshipWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameSessionService _gameSession;

        public GameController(GameSessionService gameSession)
        {
            _gameSession = gameSession;
        }

        [HttpPost("create")]
        public ActionResult<GameStateResponse> CreateGame([FromBody] CreateGameRequest request)
        {
            if (_gameSession.HasActiveGame)
            {
                return Conflict(new { error = "Game already exists. Reset first." });
            }

            if (request.PlayerNames == null || request.PlayerNames.Count != 2)
            {
                return BadRequest(new { error = "Must have exactly 2 players" });
            }

            if (request.PlayerNames.Distinct().Count() != request.PlayerNames.Count)
            {
                return BadRequest(new { error = "Player names must be unique" });
            }

            _gameSession.CreateGame(request.PlayerNames);
            var gameState = _gameSession.CurrentGame!.GetGameState();

            return Ok(gameState);
        }

        [HttpPost("reset")]
        public ActionResult Reset()
        {
            _gameSession.ResetGame();
            return Ok(new { message = "Game reset successfully" });
        }

        [HttpGet("status")]
        public ActionResult GetStatus()
        {
            return Ok(new { hasActiveGame = _gameSession.HasActiveGame });
        }

        [HttpGet("state")]
        public ActionResult<GameStateResponse> GetGameState()
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game. Create one first." });

            var gameState = _gameSession.CurrentGame!.GetGameState();
            return Ok(gameState);
        }

        [HttpPost("place-ship")]
        public ActionResult<ActionResultResponse> PlaceShip([FromBody] PlaceShipRequest request)
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game" });

            // Parse ship type
            if (!Enum.TryParse<ShipType>(request.ShipType, true, out var shipType))
            {
                return BadRequest(new { error = $"Invalid ship type: {request.ShipType}. Valid types: Carrier, Battleship, Cruiser" });
            }

            // Parse orientation
            var orientation = request.Orientation?.ToLower() == "vertical" 
                ? Orientation.Vertical 
                : Orientation.Horizontal;

            var position = new Position(request.Row, request.Col);

            var result = _gameSession.CurrentGame!.PlaceShip(request.PlayerName, shipType, position, orientation);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error?.Message });

            var dto = new ActionResultResponse
            {
                Success = result.Data,
                Message = $"Ship {shipType} placed successfully at ({request.Row}, {request.Col}) {orientation}"
            };

            return Ok(dto);
        }

        [HttpPost("start-battle")]
        public ActionResult<ActionResultResponse> StartBattle()
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game" });

            var result = _gameSession.CurrentGame!.StartBattle();
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error?.Message });

            var dto = new ActionResultResponse
            {
                Success = true,
                Message = "Battle phase started! " + _gameSession.CurrentGame.CurrentPlayer.Name + " goes first."
            };

            return Ok(dto);
        }

        [HttpPost("fire")]
        public ActionResult<FireShotResponse> Fire([FromBody] FireShotRequest request)
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game" });

            var position = new Position(request.Row, request.Col);

            var result = _gameSession.CurrentGame!.FireShot(request.PlayerName, position);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error?.Message });

            return Ok(result.Data);
        }

        [HttpPost("end-turn")]
        public ActionResult<ActionResultResponse> EndTurn([FromBody] PlayerActionRequest request)
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game" });

            var validateResult = _gameSession.CurrentGame!.ValidatePlayerTurn(request.PlayerName);
            if (!validateResult.IsSuccess)
                return BadRequest(new { error = validateResult.Error?.Message });

            _gameSession.CurrentGame.SwitchTurn();

            var dto = new ActionResultResponse
            {
                Success = true,
                Message = $"Turn ended. Now it's {_gameSession.CurrentGame.CurrentPlayer.Name}'s turn."
            };

            return Ok(dto);
        }

        [HttpGet("board/{playerName}")]
        public ActionResult<BoardResponse> GetPlayerBoard(string playerName, [FromQuery] bool hideShips = false)
        {
            if (!_gameSession.HasActiveGame)
                return NotFound(new { error = "No active game" });

            var player = _gameSession.CurrentGame!.GetPlayer(playerName);
            if (player == null)
                return NotFound(new { error = $"Player '{playerName}' not found" });

            var board = _gameSession.CurrentGame.GetBoardResponse(player, hideShips);
            return Ok(board);
        }
    }
}

