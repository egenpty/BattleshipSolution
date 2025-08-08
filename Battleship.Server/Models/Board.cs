using Microsoft.Extensions.Logging;

namespace Battleship.Server.Models;

public class Board
{
    private readonly ILogger<Board> _logger;

    public Guid Id { get; }
    public int Size { get; }

    private readonly List<Ship> _ships = new();
    private readonly HashSet<Position> _attacks = new();

    public IReadOnlyList<Ship> Ships => _ships.AsReadOnly();
    public IReadOnlyCollection<Position> Attacks => _attacks;

    public Board(int size, ILogger<Board> logger, Guid? id = null)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Board size must be greater than zero.");

        Size = size;
        Id = id ?? Guid.NewGuid();
        _logger = logger;
        _logger.LogInformation("Board created with ID {BoardId} and size {Size}", Id, Size);
    }

    public bool AddShip(Ship ship)
    {
        if (ship.Positions.Any(pos => !IsValidPosition(pos)))
        {
            _logger.LogWarning("AddShip failed: Ship positions out of board bounds on board {BoardId}", Id);
            return false;
        }

        if (_ships.Any(s => s.Positions.Intersect(ship.Positions).Any()))
        {
            _logger.LogWarning("AddShip failed: Ship overlaps with existing ship on board {BoardId}", Id);
            return false;
        }

        _ships.Add(ship);
        _logger.LogInformation("Ship added to board {BoardId} at positions {@Positions}", Id, ship.Positions);
        return true;
    }

    public AttackResult Attack(Position pos)
    {
        if (_attacks.Contains(pos))
        {
            _logger.LogInformation("Position ({X},{Y}) already attacked on board {BoardId}", pos.X, pos.Y, Id);
            return new AttackResult { Hit = false, Sunk = false };
        }

        _attacks.Add(pos);

        foreach (var ship in _ships)
        {
            if (ship.IsHit(pos))
            {
                _logger.LogInformation("Attack at ({X},{Y}) hit a ship on board {BoardId}. Ship sunk: {IsSunk}", pos.X, pos.Y, Id, ship.IsSunk);
                return new AttackResult { Hit = true, Sunk = ship.IsSunk };
            }
        }

        _logger.LogInformation("Attack at ({X},{Y}) missed on board {BoardId}", pos.X, pos.Y, Id);
        return new AttackResult { Hit = false, Sunk = false };
    }

    private bool IsValidPosition(Position pos) =>
        pos.X >= 0 && pos.Y >= 0 && pos.X < Size && pos.Y < Size;
}
