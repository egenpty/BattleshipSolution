namespace Battleship.Server.Models
{
    public class BoardCreationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Guid BoardId { get; set; }
        public int? Size { get; set; }
    }
}
