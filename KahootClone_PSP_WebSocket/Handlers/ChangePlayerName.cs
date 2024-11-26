using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public static class ChangePlayerName
{
    public static HandlerResponse Execute(Message message, KahootServer client)
    {
        Message? response = null;

        var dto = KahootDtoConvertor.ConvertToDto<ChangePlayerNameDto>(message.Data.ToString());
        var player = UnitOfWork.Instance.Players.FirstOrDefault(p => p.Id == client.ID);
        
        if (player == null)
        {
            throw new ArgumentException($"Player with id {client.ID} no found");
        }

        player.Name = dto.NewPlayerName;

        response = new Message("NameChanged", dto.NewPlayerName);

        return new HandlerResponse(response);
    }
}
