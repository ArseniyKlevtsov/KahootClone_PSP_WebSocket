namespace KahootClone_PSP_WebSocket.DTOs;

public class Message
{
    public string Command { get; set; }
    public object Data { get; set; }

    public Message(string command, object data)
    {
        Command = command;
        Data = data;
    }
}
