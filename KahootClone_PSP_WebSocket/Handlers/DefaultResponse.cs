using KahootClone_PSP_WebSocket.DTOs;

namespace KahootClone_PSP_WebSocket.Handlers;

public static class DefaultResponse
{
    public static HandlerResponse Execute(Message message)
    {
        var resposeMessage = new Message("error", $"unknown command: ({message.Command})");
        var handlerResponse = new HandlerResponse(resposeMessage);
        return handlerResponse;
    }
}
