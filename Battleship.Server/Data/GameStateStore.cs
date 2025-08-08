using Battleship.Server.Models;

namespace Battleship.Server.Data;
public class GameStateStore
{
    public Dictionary<Guid, Board> Boards { get; } = new();
}