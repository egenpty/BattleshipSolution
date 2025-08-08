namespace Battleship.Server.Models;

/// <summary>
/// Represents a 2D coordinate on the Battleship board. This class is immutable.
/// </summary>
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj) =>
        obj is Position p && p.X == X && p.Y == Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);
}
