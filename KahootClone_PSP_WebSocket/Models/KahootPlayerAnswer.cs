namespace KahootClone_PSP_WebSocket.Models;

public class KahootPlayerAnswer
{
    public string QuestionId { get; set; }
    public string PlayerId { get; set; }
    public bool Solved { get; set; }
    public int AnswerIndex { get; set; }
    public DateTime AnswerTime { get; set; }

    public KahootPlayerAnswer(string questionId, string playerId, int answerIndex)
    {
        QuestionId = questionId;
        PlayerId = playerId;
        AnswerIndex = answerIndex;
        AnswerTime = DateTime.Now;
    }
}
