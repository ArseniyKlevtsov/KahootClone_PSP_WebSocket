using KahootClone_PSP_WebSocket.Interfaces;

namespace KahootClone_PSP_WebSocket.Services;

public class ConsoleLoger : ILoger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
