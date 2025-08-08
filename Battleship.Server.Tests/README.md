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

- Validates the behavior of the `Board` class:
  - Adding ships within bounds and preventing overlaps
  - Rejecting ships outside the board boundaries
  - Tracking attacks and verifying hit/miss responses
  - Ensuring repeated attacks on the same position are handled correctly
  - Multi-step attack tests verifying sinking logic when all positions of a ship are hit

### 2. ShipTests.cs

- Validates the `Ship` class:
  - Proper initialization with fixed positions
  - Correct detection of hits on ship positions
  - Correct sinking logic when all positions are hit
  - Handling of invalid inputs during ship creation

### 3. GameServiceTests.cs

- Validates the `GameService` class, which manages game state through `GameStateStore`:
  - Board creation and ensuring boards are stored correctly
  - Adding ships to boards and verifying success/failure scenarios
  - Handling attacks through the service, returning appropriate hit/miss/null results
  - Integration with logging to verify key operations are logged (using mocked `ILogger`)
  - Multi-ship scenarios ensuring correct hit and sunk behavior across different ships

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