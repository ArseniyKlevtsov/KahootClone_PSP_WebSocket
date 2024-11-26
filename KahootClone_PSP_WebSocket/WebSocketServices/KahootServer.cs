using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.Handlers;
using KahootClone_PSP_WebSocket.Interfaces;
using KahootClone_PSP_WebSocket.Models;
using KahootClone_PSP_WebSocket.Repositories;
using KahootClone_PSP_WebSocket.Services;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KahootClone_PSP_WebSocket.WebSocketServices;

public class KahootServer : WebSocketBehavior
{
    private static ILoger loger = new ConsoleLoger();

    protected override void OnOpen()
    {
        var player = new KahootPlayer(ID);
        UnitOfWork.Instance.Players.Add(player);

        var message = new Message("PlayerJoined", player);
        SendMessage(message);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        var message = ReadMessage(e.Data);

        var handlerResponse = Router.Route(message, this);

        SendHandlerResponse(handlerResponse);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Console.WriteLine($"Client Disconected: {ID}");
    }

    private void SendMessage(Message message)
    {
        string jsonMessage = JsonConvert.SerializeObject(message);
        Send(jsonMessage);
        loger.Log($"send to {ID}: {message.Command}");
    }

    private void SendMessage(Message message, string id)
    {
        string jsonMessage = JsonConvert.SerializeObject(message);
        Sessions.SendTo(jsonMessage, id);
        loger.Log($"send to {id}: {message.Command}");
    }

    private void SendHandlerResponse(HandlerResponse response)
    {
        if (response.RecipientIds.Count > 0)
        {
            foreach (var recipientId in response.RecipientIds)
            {
                SendMessage(response.Message, recipientId);
            }
        }
        else
        {
            SendMessage(response.Message);
        }
    }

    private Message ReadMessage(string data)
    {
        Message? message;
        message = JsonConvert.DeserializeObject<Message>(data);
        if (message == null)
        {
            throw new NullReferenceException("message is null");
        }
        loger.Log($"recive from {ID}: {message.Command}");

        return message;
    }
}