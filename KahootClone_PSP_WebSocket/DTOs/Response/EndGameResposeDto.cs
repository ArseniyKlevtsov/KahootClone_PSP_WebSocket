namespace KahootClone_PSP_WebSocket.DTOs.Response;

public class EndGameResposeDto
{
    public List<PlayerStatsDto> PlayerScoreBoard { get; set; }

    public EndGameResposeDto()
    {
        PlayerScoreBoard = new List<PlayerStatsDto>();
    }
}
