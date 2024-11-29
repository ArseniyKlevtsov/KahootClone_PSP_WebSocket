using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Models;

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

    public void AddRecipientsFromRoom(KahootRoom room)
    {
        foreach (var id in room.PlayerIds)
        {
            RecipientIds.Add(id);
        }
    }
}
