namespace Battleship.Server.Models;
public class AttackResult
{
    public bool Hit { get; set; }
    public bool Sunk { get; set; }
    public string? Message { get; set; }  // Add this property

}