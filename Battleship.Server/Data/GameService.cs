using Battleship.Server.Models;
using Microsoft.Extensions.Logging;

namespace Battleship.Server.Data;

public class GameService
{
    private readonly GameStateStore _store;
    private readonly ILogger<GameService> _logger;
    private readonly ILogger<Board> _boardLogger;

    public GameService(GameStateStore store, ILogger<GameService> logger, ILogger<Board> boardLogger)
    {
        _store = store;
        _logger = logger;
        _boardLogger = boardLogger;
    }

    public Board CreateBoard(int size = 10)
    {
        var board = new Board(size, _boardLogger);
        _store.Boards[board.Id] = board;
        _logger.LogInformation("Created new board with ID {BoardId} and size {Size}", board.Id, size);
        return board;
    }

    public bool AddShip(Guid boardId, Ship ship)
    {
        if (!_store.Boards.TryGetValue(boardId, out var board))
        {
            _logger.LogWarning("AddShip failed: Board with ID {BoardId} not found", boardId);
            return false;
        }

        var result = board.AddShip(ship);
        _logger.LogInformation("AddShip on board {BoardId} was {Result}", boardId, result ? "successful" : "unsuccessful");
        return result;
    }

    public AttackResult? Attack(Guid boardId, Position pos)
    {
        if (!_store.Boards.TryGetValue(boardId, out var board))
        {
            _logger.LogWarning("Attack failed: Board with ID {BoardId} not found", boardId);
            return null;
        }

        var result = board.Attack(pos);
        _logger.LogInformation("Attack on board {BoardId} at position ({X},{Y}) resulted in Hit={Hit}, Sunk={Sunk}",
            boardId, pos.X, pos.Y, result.Hit, result.Sunk);
        return result;
    }
}
