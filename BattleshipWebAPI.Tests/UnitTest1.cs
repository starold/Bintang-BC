using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using BattleshipWeb.Services;
using BattleshipWeb.Interface;
using BattleshipWeb.Models;
using BattleshipWeb.Enums;
using BattleshipWeb.Common;
using BattleshipWeb.DTOs.Responses;
using System.Collections.Generic;


namespace BattleshipWebAPI.Tests
{
    [TestFixture]
    public class BattleshipGameServiceTests
    {
        private Mock<ILogger<GameService>> _mockLogger;
        private Mock<IPlayer> _mockPlayer1;
        private Mock<IPlayer> _mockPlayer2;
        private List<IPlayer> _players;
        private GameService _gameService;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<GameService>>();
            _mockPlayer1 = new Mock<IPlayer>();
            _mockPlayer2 = new Mock<IPlayer>();

            _mockPlayer1.Setup(p => p.Name).Returns("Player1");
            _mockPlayer2.Setup(p => p.Name).Returns("Player2");

            _players = new List<IPlayer> { _mockPlayer1.Object, _mockPlayer2.Object };

            _gameService = new GameService(_players, _mockLogger.Object);
        }

        [Test]
        public void Constructor_ShouldInitializeGame_WithCorrectState()
        {
            Assert.That(_gameService.State, Is.EqualTo(GameState.Setup));
            Assert.That(_gameService.CurrentPlayer, Is.EqualTo(_mockPlayer1.Object));
            Assert.That(_gameService.Winner, Is.Null);
            Assert.That(_gameService.GetPlayer("Player1"), Is.EqualTo(_mockPlayer1.Object));
            Assert.That(_gameService.GetPlayer("Player2"), Is.EqualTo(_mockPlayer2.Object));
        }

        [Test]
        public void ValidatePlayerTurn_ShouldReturnSuccess_WhenItIsPlayerTurn()
        {
            var result = _gameService.ValidatePlayerTurn("Player1");
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.EqualTo(_mockPlayer1.Object));
        }

        [Test]
        public void ValidatePlayerTurn_ShouldReturnFail_WhenPlayerNotFound()
        {
            var result = _gameService.ValidatePlayerTurn("UnknownPlayer");
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("not found"));
        }

        [Test]
        public void ValidatePlayerTurn_ShouldReturnFail_WhenItIsNotPlayerTurn()
        {
            var result = _gameService.ValidatePlayerTurn("Player2");
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("not Player2's turn"));
        }

        [Test]
        public void PlaceShip_ShouldReturnFail_WhenNotInSetupPhase()
        {
            // Force state to Battle
            SetupGameToBattlePhase();
            
            var result = _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(0, 0), Orientation.Horizontal);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("outside of Setup phase"));
        }

        [Test]
        public void PlaceShip_ShouldReturnFail_WhenPlayerNotFound()
        {
            var result = _gameService.PlaceShip("Unknown", ShipType.Carrier, new Position(0, 0), Orientation.Horizontal);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("not found"));
        }

        [Test]
        public void PlaceShip_ShouldReturnSuccess_WhenPlacementValid()
        {
            var result = _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(0, 0), Orientation.Horizontal);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_gameService.GetShipsPlaced(_mockPlayer1.Object), Is.EqualTo(1));
        }

        [Test]
        public void PlaceShip_ShouldReturnFail_WhenShipAlreadyPlaced()
        {
            _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(0, 0), Orientation.Horizontal);
            var result = _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(1, 0), Orientation.Horizontal);
            
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("already placed"));
        }

        [Test]
        public void PlaceShip_ShouldReturnFail_WhenPlacementOutOfBounds()
        {
            // Out of Cell
            
            var result = _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(0, 9), Orientation.Horizontal);
            Assert.That(result.IsSuccess, Is.False);
             Assert.That(result.Error.Message, Does.Contain("Invalid ship placement"));
        }

         [Test]
        public void PlaceShip_ShouldReturnFail_WhenPlacementOverlaps()
        {
            _gameService.PlaceShip("Player1", ShipType.Carrier, new Position(0, 0), Orientation.Horizontal); // 0,0 to 0,4
            
            var result = _gameService.PlaceShip("Player1", ShipType.Battleship, new Position(0, 2), Orientation.Vertical); // Intersects at 0,2
            Assert.That(result.IsSuccess, Is.False);
             Assert.That(result.Error.Message, Does.Contain("Invalid ship placement"));
        }



        [Test]
        public void StartBattle_ShouldReturnFail_WhenNotAllShipsPlaced()
        {
            var result = _gameService.StartBattle();
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("has not placed all ships"));
        }

        [Test]
        public void StartBattle_ShouldReturnSuccess_WhenAllShipsPlaced()
        {
            PlaceAllShipsForPlayer("Player1");
            PlaceAllShipsForPlayer("Player2");

            var result = _gameService.StartBattle();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_gameService.State, Is.EqualTo(GameState.Battle));
        }
    
        [Test]
        public void FireShot_ShouldReturnFail_WhenNotInBattlePhase()
        {
            var result = _gameService.FireShot("Player1", new Position(0, 0));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Is.Not.Null);
        }

        [Test]
        public void FireShot_ShouldReturnMiss_WhenNoShipHit()
        {
            SetupGameToBattlePhase();

            // Player1 shoots at Player2's board. Player2 has ships at specific locations.
            // SetupGameToBattlePhase places ships at top-left. Shooting at bottom-right should miss.
            var result = _gameService.FireShot("Player1", new Position(9, 9));
            
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data.Result, Is.EqualTo("Miss"));
            Assert.That(result.Data.IsGameOver, Is.False);
        }

        [Test]
        public void FireShot_ShouldReturnHit_WhenShipHit()
        {
            SetupGameToBattlePhase();

            // Player2 has Carrier at 0,0 Horizontal (0,0 to 0,4)
            var result = _gameService.FireShot("Player1", new Position(0, 0));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data.Result, Is.EqualTo("Hit"));
        }

        [Test]
        public void FireShot_ShouldReturnSink_WhenShipSunk()
        {
            SetupGameToBattlePhase();

            // Player2 has Carrier at 0,0 Horizontal (size 5)
            _gameService.FireShot("Player1", new Position(0, 0));
            _gameService.FireShot("Player1", new Position(0, 1));
            _gameService.FireShot("Player1", new Position(0, 2));
            // _gameService.SwitchTurn(); // Removed to ensure Player1 keeps turn to sink the ship
            
            _gameService.FireShot("Player1", new Position(0, 3));
            var result = _gameService.FireShot("Player1", new Position(0, 4));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data.Result, Is.EqualTo("Sink"));
            Assert.That(result.Data.SunkShipType, Is.EqualTo("Carrier"));
        }
        
         [Test]
        public void FireShot_ShouldReturnFail_WhenAlreadyShot()
        {
            SetupGameToBattlePhase();
            _gameService.FireShot("Player1", new Position(0, 0));
            var result = _gameService.FireShot("Player1", new Position(0, 0));

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Message, Does.Contain("already been shot"));
        }

        private void SetupGameToBattlePhase()
        {
            PlaceAllShipsForPlayer("Player1");
            PlaceAllShipsForPlayer("Player2");
            _gameService.StartBattle();
        }

        private void PlaceAllShipsForPlayer(string playerName)
        {
            // SHIPS_PER_PLAYER = 3: Carrier, Battleship, Cruiser
            _gameService.PlaceShip(playerName, ShipType.Carrier, new Position(0, 0), Orientation.Horizontal);
            _gameService.PlaceShip(playerName, ShipType.Battleship, new Position(1, 0), Orientation.Horizontal);
            _gameService.PlaceShip(playerName, ShipType.Cruiser, new Position(2, 0), Orientation.Horizontal);
        }

        
        
    }
}
