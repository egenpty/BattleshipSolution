// BoardTests.cs
using Battleship.Server.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class BoardTests
{
    /// <summary>
    /// Helper method to create a Board instance with a mocked ILogger.
    /// Allows specifying board size (default 10).
    /// </summary>
    private Board CreateBoardWithLogger(int size = 10)
    {
        var mockLogger = new Mock<ILogger<Board>>();
        return new Board(size, mockLogger.Object);
    }

    /// <summary>
    /// Helper method to create a test Ship occupying two positions (0,0) and (0,1).
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
    /// Tests that adding a valid ship within bounds succeeds.
    /// Verifies the ship is contained in the board's Ships collection.
    /// </summary>
    [Fact]
    public void AddShip_ShouldReturnTrue_WhenShipIsValid()
    {
        var board = CreateBoardWithLogger();
        var ship = CreateTestShip();

        var result = board.AddShip(ship);

        Assert.True(result);
        Assert.Contains(ship, board.Ships);
    }

    /// <summary>
    /// Tests that adding a ship placed outside the board boundaries fails.
    /// For example, position (10,10) is outside a 10x10 grid with indices 0–9.
    /// </summary>
    [Fact]
    public void AddShip_ShouldReturnFalse_WhenShipOutsideBounds()
    {
        var board = CreateBoardWithLogger();
        var ship = new Ship(new List<Position>
        {
            new Position(10, 10), // Outside 0-based 10x10 grid
        });

        var result = board.AddShip(ship);

        Assert.False(result);
    }

    /// <summary>
    /// Tests that adding a ship which overlaps an existing ship on the board fails.
    /// </summary>
    [Fact]
    public void AddShip_ShouldReturnFalse_WhenShipOverlaps()
    {
        var board = CreateBoardWithLogger();
        var ship1 = CreateTestShip();
        var ship2 = CreateTestShip();

        board.AddShip(ship1);
        var result = board.AddShip(ship2); // Same positions as ship1

        Assert.False(result);
    }

    /// <summary>
    /// Tests that an attack on a position occupied by a ship returns a hit.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnHit_WhenShipIsHit()
    {
        var board = CreateBoardWithLogger();
        var ship = CreateTestShip();
        board.AddShip(ship);

        var result = board.Attack(new Position(0, 0));

        Assert.True(result.Hit);
    }

    /// <summary>
    /// Tests that attacking the same position more than once returns a miss (Hit = false).
    /// The second attack on the same position should not register as a hit.
    /// </summary>
    [Fact]
    public void Attack_ShouldReturnFalse_WhenPositionAlreadyAttacked()
    {
        var board = CreateBoardWithLogger();
        var pos = new Position(5, 5);
        board.Attack(pos); // First attack
        var result = board.Attack(pos); // Repeat attack

        Assert.False(result.Hit);
    }

    /// <summary>
    /// Tests that attacking all positions of a ship results in the ship being sunk.
    /// Each hit should return Hit=true, and the final hit should return Sunk=true.
    /// </summary>
    [Fact]
    public void Attack_ShouldSinkShip_AfterAllPositionsHit()
    {
        var board = CreateBoardWithLogger();

        var ship = new Ship(new List<Position>
        {
            new Position(2, 3),
            new Position(3, 3),
            new Position(4, 3)
        });

        board.AddShip(ship);

        var attack1 = board.Attack(new Position(2, 3));
        Assert.True(attack1.Hit);
        Assert.False(attack1.Sunk);

        var attack2 = board.Attack(new Position(3, 3));
        Assert.True(attack2.Hit);
        Assert.False(attack2.Sunk);

        var attack3 = board.Attack(new Position(4, 3));
        Assert.True(attack3.Hit);
        Assert.True(attack3.Sunk); // Sinks the ship
    }
}
