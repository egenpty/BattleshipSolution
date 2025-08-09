# Battleship.Server.Tests

This project contains unit tests for the **Battleship.Server** core logic, focusing on validating the behavior and correctness of the main components:
`Board`, `Ship`, and `GameService`.

## Overview

The tests ensure that the Battleship game backend correctly manages:

- Board creation and state management
- Ship placement validation and collision detection
- Attack handling, including hit, miss, and sunk states
- Proper integration of the game service layer which coordinates boards and ships
- Logging verification for key operations using mocked `ILogger`
- Comprehensive test coverage including multi-step attack scenarios and multiple ships management

## Test Files and Coverage

### 1. BoardTests.cs

- Tests the core functionality of the `Board` class, including:
  - **Ship placement validation:**
    - Adding ships that fit within the board bounds succeeds.
    - Rejecting ships placed outside the board limits.
    - Preventing ship overlap on the board.
  - **Attack handling:**
    - Confirming attacks on ship positions return hits.
    - Ensuring repeated attacks on the same position do not register hits again.
    - Multi-step attacks that verify a ship is correctly marked as sunk after all its positions are hit.
  - **Logging:**  
    - Board instances are created with mocked `ILogger` to verify logging (though not shown explicitly in tests, logger injection is set up for future verifications).

#### Test Cases:

- `AddShip_ShouldReturnTrue_WhenShipIsValid`: Verifies that adding a ship within bounds succeeds and that the ship is added to the board’s collection.

- `AddShip_ShouldReturnFalse_WhenShipOutsideBounds`: Verifies that ships positioned outside the valid grid are rejected.

- `AddShip_ShouldReturnFalse_WhenShipOverlaps`: Prevents overlapping ships from being placed.

- `Attack_ShouldReturnHit_WhenShipIsHit`: Confirms an attack hitting a ship is registered correctly.

- `Attack_ShouldReturnFalse_WhenPositionAlreadyAttacked`: Ensures repeated attacks on the same coordinate do not double-count hits.

- `Attack_ShouldSinkShip_AfterAllPositionsHit`: Tests the sinking logic when all parts of a ship are hit.

---

### 2. ShipTests.cs

- Validates the `Ship` class behavior, focusing on construction and hit detection logic:

  - **Constructor validation:**
    - Throws `ArgumentNullException` when initialized with `null` positions.
    - Throws `ArgumentException` if initialized with an empty list of positions.

  - **Sinking logic:**
    - Confirms that a ship is not sunk if no positions have been hit.
    - Confirms a ship is not sunk if only some positions have been hit.
    - Confirms a ship is sunk only when all positions have been hit.

  - **Hit detection:**
    - Returns `true` if an attack matches a ship's position.
    - Returns `false` if an attack misses the ship entirely.

#### Test Cases:

- `Constructor_ShouldThrowArgumentNullException_WhenPositionsIsNull`: Validates defensive programming for null inputs.

- `Constructor_ShouldThrowArgumentException_WhenPositionsIsEmpty`: Ensures ship creation requires at least one position.

- `IsSunk_ShouldReturnFalse_WhenNoHits`: Checks initial state of a newly created ship.

- `IsSunk_ShouldReturnFalse_WhenPartiallyHit`: Verifies partial hits do not sink the ship.

- `IsSunk_ShouldReturnTrue_WhenAllPositionsHit`: Confirms sinking state when fully hit.

- `IsHit_ShouldReturnTrue_WhenAttackHitsShip`: Detects hits on ship positions.

- `IsHit_ShouldReturnFalse_WhenAttackMissesShip`: Detects misses on ship positions.

---

### 3. GameServiceTests.cs

- Tests the `GameService` class, which acts as the API layer managing game state via `GameStateStore`. Key focus areas include:

  - **Board lifecycle management:**
    - Creating new boards and confirming they are stored correctly.
    - Verifying logging calls occur during board creation.

  - **Ship management:**
    - Adding ships to existing boards with success/failure handling.
    - Ensuring ships cannot be added to non-existent boards.

  - **Attack processing:**
    - Attacks returning hits or misses depending on ship placement.
    - Returning `null` for attacks on unknown board IDs.
    - Multi-step attacks validating sinking logic when all ship positions are hit.
    - Handling multiple ships on one board with correct hit and sunk reporting.

- **Logging:**
  - Uses mocked `ILogger` instances to verify logging interactions during key operations (e.g., board creation).

#### Test Cases:

- `CreateBoard_ShouldAddNewBoardToStore_AndLog`: Confirms new boards are created, stored, and logged.

- `AddShip_ShouldReturnTrue_WhenBoardExistsAndShipIsValid`: Verifies successful ship addition to a valid board.

- `AddShip_ShouldReturnFalse_WhenBoardDoesNotExist`: Ensures adding ships to invalid boards fails gracefully.

- `Attack_ShouldReturnHit_WhenBoardExistsAndPositionHitsShip`: Confirms attacks that hit ships are identified.

- `Attack_ShouldReturnMiss_WhenBoardExistsAndPositionMissesShip`: Confirms misses when no ship occupies attacked position.

- `Attack_ShouldReturnNull_WhenBoardDoesNotExist`: Returns null for invalid board IDs.

- `Attack_ShouldReturnSunk_WhenAllPositionsOfShipAreHit`: Tests multi-attack sinking logic.

- `Attack_ShouldCorrectlyHandleMultipleShips`: Tests attack outcomes on boards with multiple ships, including hits, misses, and sinking.

---

## Testing Framework and Tools

- **xUnit**: Test framework used for writing and running unit tests.
- **Moq**: Used for mocking dependencies such as `ILogger` to isolate test behavior and verify logging.
- **Assertions**: Tests use strong assertions to verify method outcomes, internal state changes, and logging invocations.

## How to Run Tests

1. Ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.
2. Navigate to the test project directory.
3. Run the tests with the command:

   ```bash
   dotnet test
