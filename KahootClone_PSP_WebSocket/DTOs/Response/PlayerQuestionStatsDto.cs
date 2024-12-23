﻿namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class PlayerQuestionStatsDto
{
    public int TotalScore { get; set; }
    public int QuestionScore { get; set; }
    public int PlayerAnswerIndex {  get; set; }
    public bool IsCorrectAnswer { get; set; }
    public string PlayerName { get; set; }
}
