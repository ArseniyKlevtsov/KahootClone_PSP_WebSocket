using KahootClone_PSP_WebSocket.DTOs;

namespace KahootClone_PSP_WebSocket.Handlers;

public class HandlerResponse
{
    public Message Message {  get; set; }
    public List<string> RecipientIds { get; set; }
    public HandlerResponse(Message message)
    {
        Message = message;
        RecipientIds = new List<string>();
    }
}
