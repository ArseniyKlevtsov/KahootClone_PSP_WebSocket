using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.DTOs.Response;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public class NextQuestionInRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<NextQuestionInRoomDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);
        if (room == null)
        {
            throw new ArgumentException($"Room with id {client.ID} not found");
        }

        if (room.CreatorId != dto.CreatorId)
        {
            throw new ArgumentException($"Access denied");
        }

        var index = room.QuestionIndex;
        var nexQuestion = room.Questions[index];
        room.QuestionIndex++;

        var question = new QuestionResponseDto(nexQuestion.Id, nexQuestion.Question, nexQuestion.Answers);
        response = new Message("NextQuestion", question);

        var handlerResponsse = new HandlerResponse(response);
        foreach (var id in room.PlayerIds)
        {
            handlerResponsse.RecipientIds.Add(id);
        }
        handlerResponsse.RecipientIds.Add(client.ID);

        return handlerResponsse;
    }
}
