namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class QuestionStatsDto
{
    public string CorrectAnswer { get; set; }
    public List<PlayerStatsDto> PlayerScoreBoard { get; set; }

    public QuestionStatsDto(string correctAnswer)
    {
        CorrectAnswer = correctAnswer;
        PlayerScoreBoard = new List<PlayerStatsDto>();
    }
}
