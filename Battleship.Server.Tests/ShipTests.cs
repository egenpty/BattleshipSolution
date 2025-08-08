// ShipTests.cs
using Battleship.Server.Models;
using Xunit;
using System;
using System.Collections.Generic;

public class ShipTests
{
    /// <summary>
    /// Helper method to create a test ship occupying three positions.
    /// </summary>
    private Ship CreateTestShip()
    {
        return new Ship(new List<Position>
        {
            new Position(0, 0),
            new Position(0, 1),
            new Position(0, 2)
        });
    }

    /// <summary>
    /// Tests that the Ship constructor throws ArgumentNullException
    /// if null is passed as positions.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPositionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Ship(null!));
    }

    /// <summary>
    /// Tests that the Ship constructor throws ArgumentException
    /// if an empty list of positions is passed.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenPositionsIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Ship(new List<Position>()));
    }

    /// <summary>
    /// Tests that a newly created ship with no hits is not sunk.
    /// </summary>
    [Fact]
    public void IsSunk_ShouldReturnFalse_WhenNoHits()
    {
        var ship = CreateTestShip();

        Assert.False(ship.IsSunk);
    }

    /// <summary>
    /// Tests that a ship with only some positions hit is not sunk.
    /// </summary>
    [Fact]
    public void IsSunk_ShouldReturnFalse_WhenPartiallyHit()
    {
        var ship = CreateTestShip();

        // Mark one position as hit
        ship.IsHit(new Position(0, 0));

        Assert.False(ship.IsSunk);
    }

    /// <summary>
    /// Tests that a ship is sunk only when all positions have been hit.
    /// </summary>
    [Fact]
    public void IsSunk_ShouldReturnTrue_WhenAllPositionsHit()
    {
        var ship = CreateTestShip();

        // Mark all positions as hit
        foreach (var pos in ship.Positions)
            ship.IsHit(pos);

        Assert.True(ship.IsSunk);
    }

    /// <summary>
    /// Tests that IsHit returns true when the attack position
    /// matches a position occupied by the ship.
    /// </summary>
    [Fact]
    public void IsHit_ShouldReturnTrue_WhenAttackHitsShip()
    {
        var ship = CreateTestShip();

        var hitPos = new Position(0, 1);

        bool result = ship.IsHit(hitPos);

        Assert.True(result);
    }

    /// <summary>
    /// Tests that IsHit returns false when the attack position
    /// does not match any position occupied by the ship.
    /// </summary>
    [Fact]
    public void IsHit_ShouldReturnFalse_WhenAttackMissesShip()
    {
        var ship = CreateTestShip();

        var missPos = new Position(5, 5);

        bool result = ship.IsHit(missPos);

        Assert.False(result);
    }
}
