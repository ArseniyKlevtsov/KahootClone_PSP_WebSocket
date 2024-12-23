﻿namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class ConectedRoomDto
{
    public List<string> UserNames { get; set; }
    public string CreatorName { get; set; }
    public string RoomName { get; set; }
    public string ConectedPlayerName { get; set; }

    public ConectedRoomDto()
    {
        UserNames = new List<string>();
    }
}
