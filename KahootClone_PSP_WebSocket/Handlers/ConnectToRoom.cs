using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.DTOs.Response;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public static class ConnectToRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<ConnectToRoomDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);
        var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == client.ID);

        CheckRoom(room);
        CheckPlayer(player!, room!);

        player!.Score = 0;
        room!.PlayerIds.Add(client.ID);
           
        response = new Message("ConectedToRoom", CreateResponse(room, client.ID));
        var handlerResponsse = new HandlerResponse(response);
        foreach (var id in room.PlayerIds)
        {
            handlerResponsse.RecipientIds.Add(id);
        }
        handlerResponsse.RecipientIds.Add(room.CreatorId);
        return handlerResponsse;
    }

    private static void CheckRoom(KahootRoom? room)
    {
        if (room == null)
        {
            throw new ArgumentException($"Room not found");
        }

        if (room.IsOpen == false)
        {
            throw new ArgumentException($"Room {room.Name} closed for connecting");
        }
    }
    private static void CheckPlayer(KahootPlayer player, KahootRoom room)
    {
        if (player!.Name == null)
        {
            throw new ArgumentException($"Player with id {player.Id} don't have name");
        }

        var IsPlayerWithNameExist = false;

        foreach (var id in room.PlayerIds)
        {
            var p = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == id);
            if (p.Name == player.Name)
            {
                IsPlayerWithNameExist = true;
            }
        }

        if (IsPlayerWithNameExist)
        {
            throw new ArgumentException($"Player with name {player.Name} exist in room");
        }
    }

    private static ConectedRoomDto CreateResponse(KahootRoom room, string ID)
    {
        var response = new ConectedRoomDto();

        var creator = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == room.CreatorId);
        var connectedPlayer = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == ID);
        response.CreatorName = creator?.Name;
        response.ConectedPlayerName = connectedPlayer?.Name;

        response.RoomName = room.Name;

        foreach (var id in room.PlayerIds)
        {
            var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == id);
            if (player == null)
            {
                continue;
            }
            response.UserNames.Add(player.Name!);
        }
        return response;
    }
}
