namespace KahootClone_PSP_WebSocket.Models;

public class KahootRoom
{
    public string Name { get; set; }
    public string CreatorId { get; }

    public List<string> PlayerIds { get; set; }
    public List<KahootQuestion> Questions { get; set; }
    public List<KahootPlayerAnswer> PlayersAnswers { get; set; }

    public KahootRoom(string name, string creatorId)
    {
        Name = name;
        CreatorId = creatorId;
    }
}
