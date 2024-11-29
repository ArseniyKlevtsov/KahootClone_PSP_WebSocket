namespace KahootClone_PSP_WebSocket.Models;

public class KahootPlayer
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public int Score { get; set; }

    public KahootPlayer(string id)
    {
        Id = id;
    }
}
