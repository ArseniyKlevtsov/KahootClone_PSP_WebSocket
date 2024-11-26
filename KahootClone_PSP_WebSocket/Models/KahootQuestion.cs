namespace KahootClone_PSP_WebSocket.Models;

public class KahootQuestion
{
    public string Id { get; }
    public string Question { get; set; }
    public int CorrectAnswerIndex { get; set; }
    public List<string> Answers { get; set; }

    public KahootQuestion(string question, int correctAnswerIndex, List<string> answers)
    {
        Id = Guid.NewGuid().ToString();
        Question = question;
        CorrectAnswerIndex = correctAnswerIndex;
        Answers = answers;
    }
}
