﻿using KahootClone_PSP_WebSocket.DTOs;
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

        var dto = KahootDtoConvertor.ConvertToDto<OpenRoomDto>(message.Data.ToString());

        var room = UnitOfWork.Instance.Rooms.FirstOrDefault(r => r.Name == dto.RoomName);

        CheckRoom(room);
        CheckPlayer(client);

        room!.PlayerIds.Add(client.ID);

        response = new Message("ConectedToRoom", CreateResponse(room));
        var handlerResponsse = new HandlerResponse(response);
        foreach (var id in room.PlayerIds)
        {
            handlerResponsse.RecipientIds.Add(id);
        }
        return handlerResponsse;
    }

    private static void CheckRoom(KahootRoom? room)
    {
        if (room == null)
        {
            throw new ArgumentException($"Room {room.Name} not found");
        }

        if (room.IsOpen == false)
        {
            throw new ArgumentException($"Room {room.Name} closed for connecting");
        }
    }
    private static void CheckPlayer(KahootServer client)
    {
        var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == client.ID);
        if (player!.Name == null)
        {
            throw new ArgumentException($"Player with id {client.ID} don't have name");
        }
    }

    private static ConectedRoomDto CreateResponse(KahootRoom room)
    {
        var response = new ConectedRoomDto();

        var creator = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == room.CreatorId);
        response.CreatorName = creator?.Name;

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
