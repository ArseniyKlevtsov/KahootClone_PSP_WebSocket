using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public static class CreateRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<CreateRoomDto>(message.Data.ToString());

        CheckExist(dto);

        var room = new KahootRoom(dto.Name, client.ID);
        UnitOfWork.Instance.Rooms.Add(room);

        response = new Message("RoomCreated", room);

        return new HandlerResponse(response);
    }

    private static void CheckExist(CreateRoomDto dto)
    {
        if (UnitOfWork.Instance.Rooms.Any(r => r.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Room with name '{dto.Name}' already exists.");
        }
    }
}
