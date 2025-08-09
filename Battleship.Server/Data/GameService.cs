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

    public BoardCreationResponse CreateBoard(int size = 10)
    {
        if (size <= 0)
        {
            var message = $"Invalid board size: {size}. Must be greater than zero.";
            _logger.LogWarning(message);
            return new BoardCreationResponse
            {
                Success = false,
                Message = message
            };
        }

        var board = new Board(size, _boardLogger);
        _store.Boards[board.Id] = board;
        _logger.LogInformation("Created new board with ID {BoardId} and size {Size}", board.Id, size);

        return new BoardCreationResponse
        {
            Success = true,
            Message = "Board created successfully.",
            BoardId = board.Id,
            Size = board.Size
        };
    }

    public AddShipResponse AddShip(Guid boardId, Ship ship)
    {
        if (!_store.Boards.TryGetValue(boardId, out var board))
        {
            var msg = $"AddShip failed: Board with ID {boardId} not found";
            _logger.LogWarning(msg);
            return new AddShipResponse
            {
                Success = false,
                Message = msg
            };
        }

        var result = board.AddShip(ship);

        var logMsg = result
            ? $"AddShip on board {boardId} was successful"
            : $"AddShip on board {boardId} was unsuccessful";

        _logger.LogInformation(logMsg);

        return new AddShipResponse
        {
            Success = result,
            Message = result ? "Ship added successfully." : "Failed to add ship (overlap or invalid position)."
        };
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
