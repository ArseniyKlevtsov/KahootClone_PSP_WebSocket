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

        var dto = KahootDtoConvertor.ConvertToDto<ShowQuestionStatsDto>(message.Data.ToString());

        var room = GetRoomByName(dto.RoomName);
        CheckCreator(room, dto.CreatorId);

        var questionStatsDto = CreateRespose(room);
        room.QuestionIndex++;

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

        var minTime = ((DateTimeOffset)questionAnswers.Min(qa => qa.AnswerTime)).ToUnixTimeSeconds();
        var maxTime = ((DateTimeOffset)questionAnswers.Max(qa => qa.AnswerTime)).ToUnixTimeSeconds();

        var timeSpan = maxTime - minTime;

        var questionStatsDto = new QuestionStatsDto(correctAnswer);
        var playerIdsWithAnswer = new List<string>();

        foreach (var qa in questionAnswers) 
        { 
            var playerStatsDto = CreatePlayerStats(qa,question, minTime, timeSpan, playerIdsWithAnswer);
            if (playerStatsDto == null)
            {
                continue;
            }
            questionStatsDto.PlayerScoreBoard.Add(playerStatsDto);
        }

        AddPlayersWithoutAnswerToStats(questionStatsDto, room, playerIdsWithAnswer);

        questionStatsDto.PlayerScoreBoard = questionStatsDto.PlayerScoreBoard
            .OrderByDescending(ps => ps.TotalScore)
            .ToList();

        return questionStatsDto;
    }

    private static void AddPlayersWithoutAnswerToStats(QuestionStatsDto questionStatsDto, KahootRoom room, List<string> playerIdsWithAnswer)
    {
        foreach (var pId in room.PlayerIds)
        {
            if (playerIdsWithAnswer.Contains(pId))
            {
                continue;
            }

            var playerStats = new PlayerQuestionStatsDto();
            var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == pId);
            if (player == null)
            {
                continue;
            }
            playerStats.PlayerName = player.Name!;
            playerStats.PlayerAnswerIndex = -1;
            playerStats.IsCorrectAnswer = false;
            playerStats.QuestionScore = 0;
            playerStats.TotalScore = player.Score;

            questionStatsDto.PlayerScoreBoard.Add(playerStats);
        }
    }

    private static PlayerQuestionStatsDto? CreatePlayerStats(
        KahootPlayerAnswer answer,
        KahootQuestion question,
        long minTime,
        long timeSpan,
        List<string> playerIdsWithAnswer)
    {
        double coef = 1;
        var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == answer.PlayerId);
        if (player == null) 
        {
            return null;
        }
        var playerStats = new PlayerQuestionStatsDto();

        playerIdsWithAnswer.Add(answer.PlayerId);

        playerStats.PlayerName = player.Name!;
        playerStats.PlayerAnswerIndex = answer.AnswerIndex;

        if (timeSpan != 0)
        {
            var timeDifference = ((DateTimeOffset)answer.AnswerTime).ToUnixTimeSeconds() - minTime;
            coef = 1 - timeDifference / timeSpan;
        }

        if (answer.AnswerIndex == question.CorrectAnswerIndex) 
        {
            answer.Solved = true;
            playerStats.IsCorrectAnswer = true;
        }

        if(playerStats.IsCorrectAnswer)
        {
            playerStats.QuestionScore = 500 + (int)Math.Round(500 * coef);
            player.Score += playerStats.QuestionScore;
            playerStats.TotalScore = player.Score;
        }

        return playerStats;
    }
}
