using KahootClone_PSP_WebSocket.DTOs;
using KahootClone_PSP_WebSocket.WebSocketServices;

namespace KahootClone_PSP_WebSocket.Handlers;

public static class Router
{ 
    public static HandlerResponse Route(Message message, KahootServer client)
    {
        HandlerResponse? response = null;
        switch (message.Command)
        {
            case "ChangePlayerName":
                response = ChangePlayerName.Execute(message, client);
                break;
            case "CreateRoom":
                response = CreateRoom.Execute(message, client);
                break;
            case "AddQuestionToRoom":
                response = AddQuestionToRoom.Execute(message, client);
                break;
            case "OpenRoom":
                response = OpenRoom.Execute(message, client);
                break;
            case "ConnectToRoom":
                response = ConnectToRoom.Execute(message, client);
                break;
            case "StartGameInRoom":
                break;
            case "NextQuestionInRoom":
                break;
            case "GetQuestionStats":
                break;
            case "GetGameStats":
                break;
            default:
                response = DefaultResponse.Execute(message);
                break;
        }
        return response!;
    }
}
