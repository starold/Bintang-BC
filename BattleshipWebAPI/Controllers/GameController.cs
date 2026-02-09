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
        // Static field to hold single game instance (no DI, no lock)
        private static GameService? _currentGame;

        [HttpPost("create")]
        public ActionResult<GameStateResponse> CreateGame([FromBody] CreateGameRequest request)
        {
            if (_currentGame != null)
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

            _currentGame = GameInitializationService.CreateGame(request.PlayerNames);
            var gameState = _currentGame.GetGameState();

            return Ok(gameState);
        }

        [HttpPost("reset")]
        public ActionResult Reset()
        {
            _currentGame = null;
            return Ok(new { message = "Game reset successfully" });
        }

        [HttpGet("status")]
        public ActionResult GetStatus()
        {
            return Ok(new { hasActiveGame = _currentGame != null });
        }

        [HttpGet("state")]
        public ActionResult<GameStateResponse> GetGameState()
        {
            if (_currentGame == null)
                return NotFound(new { error = "No active game. Create one first." });

            var gameState = _currentGame.GetGameState();
            return Ok(gameState);
        }

        [HttpPost("place-ship")]
        public ActionResult<ActionResultResponse> PlaceShip([FromBody] PlaceShipRequest request)
        {
            if (_currentGame == null)
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

            var result = _currentGame.PlaceShip(request.PlayerName, shipType, position, orientation);
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
            if (_currentGame == null)
                return NotFound(new { error = "No active game" });

            var result = _currentGame.StartBattle();
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error?.Message });

            var dto = new ActionResultResponse
            {
                Success = true,
                Message = "Battle phase started! " + _currentGame.CurrentPlayer.Name + " goes first."
            };

            return Ok(dto);
        }

        [HttpPost("fire")]
        public ActionResult<FireShotResponse> Fire([FromBody] FireShotRequest request)
        {
            if (_currentGame == null)
                return NotFound(new { error = "No active game" });

            var position = new Position(request.Row, request.Col);

            var result = _currentGame.FireShot(request.PlayerName, position);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error?.Message });

            return Ok(result.Data);
        }

        [HttpPost("end-turn")]
        public ActionResult<ActionResultResponse> EndTurn([FromBody] PlayerActionRequest request)
        {
            if (_currentGame == null)
                return NotFound(new { error = "No active game" });

            var validateResult = _currentGame.ValidatePlayerTurn(request.PlayerName);
            if (!validateResult.IsSuccess)
                return BadRequest(new { error = validateResult.Error?.Message });

            _currentGame.SwitchTurn();

            var dto = new ActionResultResponse
            {
                Success = true,
                Message = $"Turn ended. Now it's {_currentGame.CurrentPlayer.Name}'s turn."
            };

            return Ok(dto);
        }

        [HttpGet("board/{playerName}")]
        public ActionResult<BoardResponse> GetPlayerBoard(string playerName, [FromQuery] bool hideShips = false)
        {
            if (_currentGame == null)
                return NotFound(new { error = "No active game" });

            var player = _currentGame.GetPlayer(playerName);
            if (player == null)
                return NotFound(new { error = $"Player '{playerName}' not found" });

            var board = _currentGame.GetBoardResponse(player, hideShips);
            return Ok(board);
        }
    }
}
