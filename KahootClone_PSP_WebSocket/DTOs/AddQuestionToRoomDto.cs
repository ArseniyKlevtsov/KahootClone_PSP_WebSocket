namespace KahootClone_PSP_WebSocket.DTOs;

public class AddQuestionToRoomDto
{
    public string Question { get; set; }
    public int CorrectAnswerIndex { get; set; }
    public List<string> Answers { get; set; }
}
