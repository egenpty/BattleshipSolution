// GameServiceTests.cs
using Battleship.Server.Data;
using Battleship.Server.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;

public class GameServiceTests
{
    /// <summary>
    /// Helper method to create a new GameStateStore instance.
    /// </summary>
    private GameStateStore CreateStore() => new();

    /// <summary>
    /// Helper method to create a GameService instance with mocked loggers.
    /// Optionally accepts an existing GameStateStore.
    /// </summary>
    private GameService CreateService(GameStateStore? store = null)
    {
        var actualStore = store ?? CreateStore();
        var logger = new Mock<ILogger<GameService>>();
        var boardLogger = new Mock<ILogger<Board>>();
        return new GameService(actualStore, logger.Object, boardLogger.Object);
    }

    /// <summary>
    /// Helper method to create a Ship occupying two fixed positions.
    /// </summary>
    private Ship CreateTestShip()
    {
        return new Ship(new List<Position>
        {
            new Position(0, 0),
            new Position(0, 1)
        });
    }

    /// <summary>
    /// Tests that CreateBoard creates a new board, adds it to the store,
    /// and logs the creation event.
    /// </summary>
    [Fact]
    public void CreateBoard_ShouldAddNewBoardToStore_AndLog()
    {
        var store = CreateStore();
        var mockLogger = new Mock<ILogger<GameService>>();
        var mockBoardLogger = new Mock<ILogger<Board>>();
        var service = new GameService(store, mockLogger.Object, mockBoardLogger.Object);

        var board = service.CreateBoard();

        // Assert board is created and added to the store
        Assert.NotNull(board);
        Assert.Contains(board.BoardId, store.Boards.Keys);

        // Verify that an informational log was written during board creation
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Created new board")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that AddShip returns true when the specified board exists
    /// and the ship can be added successfully.
    /// </summary>
    [Fact]
    public void AddShip_ShouldReturnSuccess_WhenBoardExistsAndShipIsValid()
    {
        var store = CreateStore();
        var service = CreateService(store);
        var board = service.CreateBoard();
        var ship = CreateTestShip();

        var response = service.AddShip(board.BoardId, ship);

        // Assert the ship was successfully added
        Assert.True(response.Success, "Expected AddShip to succeed.");
        Assert.Contains(ship, store.Boards[board.BoardId].Ships);
    }

    /// <summary>
    /// Tests that AddShip returns failure when the board with the specified ID does not exist.
    /// </summary>
    [Fact]
    public void AddShip_ShouldReturnFailure_WhenBoardDoesNotExist()
    {
        var service = CreateService();
        var ship = CreateTestShip();
        var nonExistentBoardId = Guid.NewGuid();

        var response = service.AddShip(nonExistentBoardId, ship);

        Assert.False(response.Success, "Expected AddShip to fail when board does not exist.");
        Assert.Equal($"AddShip failed: Board with ID {nonExistentBoardId} not found", response.Message);
    }

    /// <summary>
    /// Tests that Attack returns a hit result when the attack position
    /// matches a position occupied by a ship on the board.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnHit_WhenBoardExistsAndPositionHitsShip()
    {
        var store = CreateStore();
        var service = CreateService(store);
        var board = service.CreateBoard();
        var ship = CreateTestShip();
        service.AddShip(board.BoardId, ship);

        var result = service.Attack(board.BoardId, new Position(0, 0));

        Assert.NotNull(result);
        Assert.True(result!.Hit);
    }

    /// <summary>
    /// Tests that Attack returns a miss result when the attack position
    /// does not match any ship's position on the board.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnMiss_WhenBoardExistsAndPositionMissesShip()
    {
        var store = CreateStore();
        var service = CreateService(store);
        var board = service.CreateBoard();

        var result = service.Attack(board.BoardId, new Position(5, 5));

        Assert.NotNull(result);
        Assert.False(result!.Hit);
    }

    /// <summary>
    /// Tests that Attack returns null when the board with the specified ID
    /// does not exist in the store.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnNull_WhenBoardDoesNotExist()
    {
        var service = CreateService();

        var result = service.Attack(Guid.NewGuid(), new Position(0, 0));

        Assert.Null(result);
    }

    /// <summary>
    /// Tests a multi-step attack sequence on the same ship to confirm hit, hit, then sunk.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnSunk_WhenAllPositionsOfShipAreHit()
    {
        var store = CreateStore();
        var service = CreateService(store);
        var board = service.CreateBoard();

        // Add a 3-cell ship horizontally at y=3
        var ship = new Ship(new List<Position>
        {
            new Position(2, 3),
            new Position(3, 3),
            new Position(4, 3)
        });

        service.AddShip(board.BoardId, ship);

        // Step-by-step attacks
        var result1 = service.Attack(board.BoardId, new Position(2, 3));
        var result2 = service.Attack(board.BoardId, new Position(3, 3));
        var result3 = service.Attack(board.BoardId, new Position(4, 3));

        Assert.NotNull(result1);
        Assert.True(result1!.Hit);
        Assert.False(result1.Sunk); // Not yet sunk

        Assert.NotNull(result2);
        Assert.True(result2!.Hit);
        Assert.False(result2.Sunk); // Still not sunk

        Assert.NotNull(result3);
        Assert.True(result3!.Hit);
        Assert.True(result3.Sunk); // Now the ship is sunk
    }

    /// <summary>
    /// Tests attacking multiple ships and verifying hits, misses, and sunk status.
    /// </summary>
    [Fact]
    public void Attack_ShouldCorrectlyHandleMultipleShips()
    {
        var store = CreateStore();
        var service = CreateService(store);
        var board = service.CreateBoard();

        // Add two ships
        var ship1 = new Ship(new List<Position> // vertical ship
        {
            new Position(1, 1),
            new Position(1, 2)
        });

        var ship2 = new Ship(new List<Position> // horizontal ship
        {
            new Position(5, 5),
            new Position(6, 5),
            new Position(7, 5)
        });

        service.AddShip(board.BoardId, ship1);
        service.AddShip(board.BoardId, ship2);

        // Attack all positions
        var r1 = service.Attack(board.BoardId, new Position(1, 1)); // hit
        var r2 = service.Attack(board.BoardId, new Position(1, 2)); // sunk
        var r3 = service.Attack(board.BoardId, new Position(5, 5)); // hit
        var r4 = service.Attack(board.BoardId, new Position(6, 5)); // hit
        var r5 = service.Attack(board.BoardId, new Position(7, 5)); // sunk
        var r6 = service.Attack(board.BoardId, new Position(9, 9)); // miss

        Assert.True(r1!.Hit);
        Assert.False(r1.Sunk);

        Assert.True(r2!.Hit);
        Assert.True(r2.Sunk); // ship1 should be sunk

        Assert.True(r3!.Hit);
        Assert.False(r3.Sunk);

        Assert.True(r4!.Hit);
        Assert.False(r4.Sunk);

        Assert.True(r5!.Hit);
        Assert.True(r5.Sunk); // ship2 should be sunk

        Assert.False(r6!.Hit);
    }

}
