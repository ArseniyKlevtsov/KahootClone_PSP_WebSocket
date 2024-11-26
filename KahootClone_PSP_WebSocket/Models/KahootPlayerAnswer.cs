namespace KahootClone_PSP_WebSocket.Models;

public class KahootPlayerAnswer
{
    public string QuestionId { get; }
    public string PlayerId { get; }
    public bool Solved { get; }
    public int AnswerIndex { get; }

    public KahootPlayerAnswer(string questionId, string playerId, int answerIndex)
    {
        QuestionId = questionId;
        PlayerId = playerId;
        AnswerIndex = answerIndex;
    }
}
