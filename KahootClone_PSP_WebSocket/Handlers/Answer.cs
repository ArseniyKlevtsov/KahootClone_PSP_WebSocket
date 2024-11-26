using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.DTOs.Response;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public class Answer
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<AnswerDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);
        CheckRoom(room);
        CheckClient(room!, client.ID);

        var solved = IsCorrectAnswer(dto, room!);

        var kahootPlayerAnswer = new KahootPlayerAnswer(dto.QuestionId, client.ID, dto.AnswerIndex);
        room!.PlayersAnswers.Add(kahootPlayerAnswer);

        response = new Message("AnswerSaved", kahootPlayerAnswer);

        return new HandlerResponse(response);
    }

    private static void CheckClient(KahootRoom room, string clientId)
    {
        var isExisct = false;
        foreach(var id in room.PlayerIds)
        {
            if(id == clientId)
            {
                isExisct = true;
            }
        }

        if(!isExisct)
        {
            throw new Exception("Access denied");
        }
    }

    private static bool IsCorrectAnswer(AnswerDto dto, KahootRoom room)
    {
        var question = room.Questions.FirstOrDefault(q => q.Id == dto.QuestionId);

        if(question == null)
        {
            throw new ArgumentException($"quetion with id {dto.QuestionId} no found");
        }

        return question.CorrectAnswerIndex == dto.AnswerIndex;
    }

    private static void CheckRoom(KahootRoom? room)
    {
        if (room == null)
        {
            throw new ArgumentException($"Room {room.Name} not found");
        }
    }
}
