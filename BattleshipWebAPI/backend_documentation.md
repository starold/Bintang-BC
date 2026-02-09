# Battleship Web API Documentation

## 1. Overview
The **Battleship Web API** is a .NET Core Web API that serves as the backend for a Battleship game. It manages game state, player sessions, ship placement, and battle logic. The API is designed to support a frontend application (e.g., React) by exposing RESTful endpoints.

**Current State**: The application uses an **in-memory** state management approach (static variable) to hold a single active game instance. This is suitable for development or single-session testing but would need a persistent database (e.g., SQL Server, Redis) for production scaling.

## 2. Architecture
The project follows a standard **Controller-Service-Model** architecture:

*   **Controllers (`Controllers/`)**: Handle HTTP requests, parsing input (DTOs), and returning HTTP responses. They delegate business logic to Services.
*   **Services (`Services/`)**: Contain the core business logic (rules of the game, state transitions, validation).
*   **Models (`Models/`)**: Represent the core domain entities (Board, Ship, Player).
*   **DTOs (`DTOs/`)**: Data Transfer Objects used to define the API contract (Requests and Responses) to decouple the internal domain model from the external API.
*   **Interfaces (`Interface/`)**: Define contracts for core components to allow for dependency injection and testing/mocking.
*   **Enums (`Enum/`)**: Define fixed sets of values like `ShipType`, `GameState`, `Orientation`.

## 3. Core Logic & Game Rules

### 3.1. Game Lifecycle
The game progresses through three states (defined in `GameState` enum):
1.  **Setup**: Players are created. Ships must be placed on the board.
2.  **Battle**: Players take turns firing shots at the opponent's board.
3.  **Finished**: One player has sunk all opponent ships.

### 3.2. Ship Placement Logic
*   **Class**: `GameService`
*   **Method**: `PlaceShip` & `IsValidShipPlacement`
*   **Rules**:
    *   Ships can only be placed during the `Setup` phase.
    *   Ships can be placed Horizontally or Vertically.
    *   **Boundary Check**: The ship must fit within the 10x10 grid (A-J, 0-9).
    *   **Overlap Check**: A ship cannot overlap with any existing ship on the board.
    *   **Uniqueness**: A player cannot place the same `ShipType` more than once.
    *   **Completion**: The game cannot proceed to `Battle` phase until both players have placed all 3 required ships (Carrier, Battleship, Cruiser).

### 3.3. Battle Logic (Firing)
*   **Class**: `GameService`
*   **Method**: `FireShot`
*   **Rules**:
    *   **Turn Validation**: Players must take turns. If it is Player A's turn, Player B cannot fire.
    *   **Coordinate Validation**: Shots must be within the board boundaries.
    *   **Duplicate Shot Check**: Players cannot fire at a coordinate that has already been shot.
    *   **Hit/Miss/Sink**:
        *   **Miss**: Cell has no ship.
        *   **Hit**: Cell has a ship. The ship's `HitCount` is incremented.
        *   **Sink**: If a ship's `HitCount` equals its `Size`, it is sunk.
    *   **Win Condition**: The game checks if all ships of the opponent are sunk. If so, the game state changes to `Finished`, and the current player is declared the winner.

## 4. Class & Method Detail

### 4.1. Controllers

#### `GameController`
The entry point for all API interactions.
*   `POST /create`: Initializes a new game with 2 players.
*   `POST /reset`: Clears the current game instance.
*   `GET /status`: Checks if a game exists.
*   `GET /state`: Returns the full current state of the game (turn, phase, players).
*   `POST /place-ship`: Places a ship for a specific player.
*   `POST /start-battle`: Transitions the game from `Setup` to `Battle` phase (requires all ships placed).
*   `POST /fire`: Processes a shot from the current player.
*   `POST /end-turn`: Explicitly ends a turn (though `fire` often handles flow, this might be for specific client-side mechanics).
*   `GET /board/{playerName}`: Retrieves the board for a specific player (can hide ships for opponent view).

### 4.2. Services

#### `GameService`
The "Brain" of the application.
*   **Properties**:
    *   `CurrentPlayer`: Tracks whose turn it is.
    *   `State`: Current `GameState` (Setup, Battle, Finished).
    *   `Winner`: Valid only when State is Finished.
*   **Methods**:
    *   `ValidatePlayerTurn(string playerName)`: returning generic `ServiceResult`, checks if the request comes from the correct active player.
    *   `PlaceShip(...)`: Orchestrates ship placement, calling `IsValidShipPlacement`.
    *   `IsValidShipPlacement(...)`: Private helper. Checks array bounds and collision with existing `board.Cells`.
    *   `StartBattle()`: Verifies ship counts (3 per player) and switches state to Battle.
    *   `FireShot(...)`: Main gameplay loop. Handles cell updates, hit detection, and calling `CheckWinner`.
    *   `CheckWinner()`: Iterates over the opponent's ships to see if all are sunk.
    *   `GetGameState()`: Maps internal domain models to `GameStateResponse` DTO for the client.
    *   `GetBoardResponse(...)`: Maps internal `Board` matrix to `BoardResponse` DTO. Handles "Fog of War" (hiding un-hit ships) via the `hideShips` flag.

#### `GameInitializationService`
*   **Purpose**: Factory to create a customized `GameService` instance.
*   **Method**: `CreateGame(List<string> names)`: Creates `Player` objects and passes them to the `GameService` constructor.

### 4.3. Models

#### `Board`
*   Represents the 10x10 grid.
*   **Data**: `Cell[,] Cells` (2D array).
*   **Constructor**: Initializes all cells with empty state.

#### `Cell`
*   Represents a single coordinate (Row, Col).
*   **Properties**:
    *   `IShip? Ship`: Reference to a ship if one occupies this cell.
    *   `bool IsShot`: True if this cell has been targeted.

#### `Ship`
*   **Properties**:
    *   `ShipType`: Enum (Carrier, Battleship, Cruiser).
    *   `Size`: Int, derived from `ShipSize` enum.
    *   `HitCount`: Tracks damage.

#### `Player`
*   **Properties**: `Name`. Simple identity class.

#### `Position`
*   **Purpose**: Value object for (Row, Column) coordinates.
*   **Helpers**: `Parse(string input)` to convert "A5" to `(0, 5)`.

### 4.4. Common & Utilities

#### `ServiceResult<T>`
*   A wrapper pattern used by `GameService` to return either Data (Success) or an Error (Fail) without throwing Exceptions. This allows the Controller to decide appropriately between HTTP 200, 400, or 404 responses.

## 5. API Data Structures (DTOs)

The API uses specific shapes for communication:

*   **Response Objects**:
    *   `GameStateResponse`: The "Big Picture" state designed for the frontend to render the top status bar (Phase, Turn, Winner).
    *   `BoardResponse`: Nested list of `CellResponse` to render the grid.
    *   `CellResponse`: `IsShot`, `HasShip` (for rendering hits/misses).
*   **Request Objects**:
    *   `PlaceShipRequest`: `{ PlayerName, ShipType, Row, Col, Orientation }`
    *   `FireShotRequest`: `{ PlayerName, Row, Col }`
