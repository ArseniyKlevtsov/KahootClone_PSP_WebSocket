using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.DTOs.Response;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

internal class EndGameInRoom
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<EndGameInRoomDto>(message.Data.ToString());

        var room = GetRoomByName(dto.RoomName);
        CheckCreator(room, dto.CreatorId);

        var endGameResponseDto = CreateRespose(room);

        response = new Message("EndGameStats", endGameResponseDto);

        var handlerResponsse = new HandlerResponse(response);
        handlerResponsse.AddRecipientsFromRoom(room);
        handlerResponsse.RecipientIds.Add(client.ID);

        return handlerResponsse;
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

    private static void CheckCreator(KahootRoom room, string creatorId)
    {
        if (room.CreatorId != creatorId)
        {
            throw new ArgumentException($"Access denied");
        }
    }

    private static EndGameResposeDto CreateRespose(KahootRoom room)
    {
        var endGameResposeDto = new EndGameResposeDto();
        foreach (var pId in room.PlayerIds)
        {
            var playerStats = new PlayerStatsDto();
            var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == pId);
            if (player == null)
            {
                continue;
            }
            playerStats.PlayerName = player.Name!;
            playerStats.TotalScore = player.Score;
            endGameResposeDto.PlayerScoreBoard.Add(playerStats);
        }

        endGameResposeDto.PlayerScoreBoard = endGameResposeDto.PlayerScoreBoard
            .OrderByDescending(ps => ps.TotalScore)
            .ToList();

        return endGameResposeDto;
    }
}
