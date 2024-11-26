namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class QuestionResponseDto
{

    public string Id { get; }
    public string Question { get; set; }
    public List<string> Answers { get; set; }

    public QuestionResponseDto(string id, string question, List<string> answers)
    {
        Id = id;
        Question = question;
        Answers = answers;
    }
}
