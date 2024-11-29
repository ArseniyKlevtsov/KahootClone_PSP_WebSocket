using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.DTOs.Response;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public class ShowQuestionStats
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<NextQuestionInRoomDto>(message.Data.ToString());

        var room = GetRoomByName(dto.RoomName);
        CheckCreator(room, dto.CreatorId);

        var questionStatsDto = CreateRespose(room);

        response = new Message("QuestionStats", questionStatsDto);

        var handlerResponsse = new HandlerResponse(response);
        handlerResponsse.AddRecipientsFromRoom(room);
        handlerResponsse.RecipientIds.Add(client.ID);

        return handlerResponsse;
    }

    private static void CheckCreator(KahootRoom room, string creatorId)
    {
        if (room.CreatorId != creatorId)
        {
            throw new ArgumentException($"Access denied");
        }
    }

    private static KahootRoom GetRoomByName(string name)
    {
        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == name);
        if (room == null)
        {
            throw new ArgumentException($"Room with name {name} not found");
        }
        return room;
    }

    private static QuestionStatsDto CreateRespose(KahootRoom room)
    {
        var question = room.Questions[room.QuestionIndex];
        var correctAnswer = question.Answers[question.CorrectAnswerIndex];

        var questionAnswers = room.PlayersAnswers.Where(pa => pa.QuestionId == question.Id);

        var minTime = questionAnswers.Min(qa => qa.AnswerTime);
        var maxTime = questionAnswers.Max(qa => qa.AnswerTime);

        var timeSpan = maxTime - minTime;
        var maxAnswerTime = timeSpan.TotalMilliseconds;

        var QuestionResponseDto = new QuestionStatsDto(correctAnswer);

        foreach (var qa in questionAnswers) 
        { 
            var playerStatsDto = CreatePlayerStats(qa,question, minTime, maxAnswerTime);
            if (playerStatsDto == null)
            {
                continue;
            }
            QuestionResponseDto.PlayerScoreBoard.Add(playerStatsDto);
        }

        QuestionResponseDto.PlayerScoreBoard = QuestionResponseDto.PlayerScoreBoard
            .OrderByDescending(ps => ps.TotalScore)
            .ToList();

        return QuestionResponseDto;
    }

    private static PlayerStatsDto? CreatePlayerStats(KahootPlayerAnswer answer, KahootQuestion question, DateTime minTime, double maxAnswerTime)
    {
        double coef = 1;
        var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == answer.PlayerId);
        if (player == null) 
        {
            return null;
        }
        var playerStats = new PlayerStatsDto();


        playerStats.PlayerName = player.Name!;
        playerStats.PlayerAnswer = question.Answers[answer.AnswerIndex];

        if (maxAnswerTime != 0)
        {
            var timeDifference = answer.AnswerTime - minTime;
            coef = 1 - timeDifference.TotalMicroseconds / maxAnswerTime;
        }

        if (answer.AnswerIndex == question.CorrectAnswerIndex) 
        {
            answer.Solved = true;
            playerStats.IsCorrectAnswer = true;
        }

        playerStats.QuestionScore = (int)Math.Round(1000 * coef);
        player.Score += playerStats.QuestionScore;
        playerStats.TotalScore = player.Score;
        return playerStats;
    }
}
