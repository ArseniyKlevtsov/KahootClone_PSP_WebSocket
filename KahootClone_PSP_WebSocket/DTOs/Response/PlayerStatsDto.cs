namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class PlayerStatsDto
{
    public int TotalScore { get; set; }
    public int QuestionScore { get; set; }
    public string PlayerAnswer {  get; set; }
    public bool IsCorrectAnswer { get; set; }
    public string PlayerName { get; set; }
}
