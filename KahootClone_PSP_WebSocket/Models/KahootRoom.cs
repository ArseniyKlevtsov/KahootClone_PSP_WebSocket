namespace KahootClone_PSP_WebSocket.Models;

public class KahootRoom
{
    public string Name { get; set; }
    public string CreatorId { get; set; }
    public bool IsOpen { get; set; }
    public int QuestionIndex {  get; set; }

    public List<string> PlayerIds { get; set; }
    public List<KahootQuestion> Questions { get; set; }
    public List<KahootPlayerAnswer> PlayersAnswers { get; set; }

    public KahootRoom(string name, string creatorId)
    {
        Name = name;
        CreatorId = creatorId;
        PlayerIds = new List<string>();
        Questions = new List<KahootQuestion>();
        PlayersAnswers = new List<KahootPlayerAnswer>();
        IsOpen = false;
        QuestionIndex = 0;
    }
}
