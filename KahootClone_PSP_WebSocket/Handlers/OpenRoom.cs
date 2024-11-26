using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public class OpenRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<OpenRoomDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);
        if (room == null)
        {
            throw new ArgumentException($"Room with id {client.ID} not found");
        }

        if(room.CreatorId != dto.CreatorId)
        {
            throw new ArgumentException($"Access denied");
        }

        room.IsOpen = true;

        response = new Message("RoomOpenned", room);
        return new HandlerResponse(response);
    }
}
