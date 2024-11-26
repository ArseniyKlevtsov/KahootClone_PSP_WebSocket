using KahootClone_PSP_WebSocket.Models;
using System.Collections.Concurrent;

namespace KahootClone_PSP_WebSocket.Repositories;

public class UnitOfWork
{
    private static readonly Lazy<UnitOfWork> instance = new Lazy<UnitOfWork>(() => new UnitOfWork());

    public static UnitOfWork Instance => instance.Value;

    public ConcurrentBag<KahootPlayer> Players { get; private set; }
    public ConcurrentBag<KahootRoom> Rooms { get; private set; }

    private UnitOfWork()
    {
        Players = new ConcurrentBag<KahootPlayer>();
        Rooms = new ConcurrentBag<KahootRoom>();
    }

}
