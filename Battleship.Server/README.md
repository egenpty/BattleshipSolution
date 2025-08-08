# Battleship State Tracker API

    Overview
    This project implements a Battleship game state tracker API in C# using ASP.NET Core. 
    It focuses on managing the game state for a single player board — creating boards, placing ships, and processing attacks.

    No UI or persistent storage is included; the API manages state in-memory. 
    This project demonstrates modern coding practices, clean architecture, logging, and automated testing.

## Features

    - Create a 10x10 Battleship board (configurable size)
    - Add ships with horizontal or vertical placement; validation prevents overlap or out-of-bounds placement
    - Process attacks on board positions, returning hit/miss and sunk ship status
    - Maintain immutable, encapsulated data structures for positions, ships, and attacks
    - Comprehensive unit tests covering core game logic and edge cases
    - Structured logging at both service and domain model layers for observability
    - Configuration using environment-specific JSON files for production readiness

## Project Structure

    - Models
        Contains core domain models: Board, Ship, Position, AttackResult
        Encapsulates validation, hit detection, and state mutation logic
    - Data
        Contains GameService and in-memory GameStateStore managing multiple boards
    - Tests
        Unit tests for GameService and domain logic using xUnit
    - Program.cs
        Application startup, dependency injection, logging configuration

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 or later (with ASP.NET and web development workload)

### Running the Project

1. Clone or download the repository
2. Open `BattleshipSolution.sln` or `Battleship.Server\Battleship.Server.csproj` in Visual Studio 2022
3. Build and run the project (F5)
4. The app will launch at `http://localhost:{port}` (port will vary)

### Testing the API (Swagger)

Swagger UI is available at:

http://localhost:{port}/swagger


Replace `{port}` with the port shown in your console or Visual Studio output.

## Configuration

    Logging and environment settings are managed via:
        appsettings.json — base config
        appsettings.Development.json — dev overrides (verbose logging)
        appsettings.Production.json — production overrides (reduced logging)

    Set environment variable ASPNETCORE_ENVIRONMENT to switch config:
        Development for local dev
        Production for production environment

## Logging

    - Uses ILogger injected into GameService and Board for detailed event logging
    - Logs include board creation, ship placements, attacks, hits, misses, and errors
    - By default logs to console; configuration can be extended for file or centralized logging

## Testing

    - Board creation (BoardTests and GameServiceTests)
    - Ship placement validation (BoardTests and ShipTests)
    - Attack results and sunk ship detection (BoardTests, ShipTests, and GameServiceTests)
    - Edge cases like repeated attacks and invalid positions (BoardTests and indirectly in service tests)

## Available endpoints:

- `POST /api/board`  
    Creates a new board. Response includes a unique board `Id`.

- `POST /api/board/{id}/ship`  
    Add a battleship to the board with JSON body specifying positions.  
    Example:

      ```json
      {
        "positions": [
          { "x": 2, "y": 3 },
          { "x": 3, "y": 3 },
          { "x": 4, "y": 3 }
        ]
      }

- `POST /api/board/{id}/attack`
    Attack a position on the board.
    Example request body:

    {
      "x": 2,
      "y": 3
    }

## Next Steps / Improvements

    - Add persistence layer (database) to retain state between sessions
    - Build a frontend UI (Blazor or SPA) to interact with the API visually
    - Add support for multiplayer turn tracking and full game lifecycle
    - Enhance input validation and error handling with detailed responses
    - Integrate structured logging to files or external monitoring systems
    - Add async API support for scalability

## Contact

    For questions or feedback, please contact:
    Shanu Gulati – shanugulati@gmail.com