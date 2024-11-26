using KahootClone_PSP_WebSocket.WebSocketServices;
using WebSocketSharp.Server;

namespace KahootClone_PSP_WebSocket;

internal class Program
{
    static void Main(string[] args)
    {
        var wssv = new WebSocketServer("ws://127.0.0.1:7890");
        wssv.AddWebSocketService<KahootServer>("/kahoot");
        wssv.Start();
        Console.WriteLine("WebSocket сервер запущен на ws://127.0.0.1:7890/kahoot");
        Console.WriteLine("Нажмите Enter для выхода...");
        Console.ReadLine();
        wssv.Stop();
    }
}
