using System;
using Newtonsoft.Json.Linq;

public class DailyMissionResponse
{
    public string message;
    public long idDailyMission;
    public bool isSuccess;
    public byte[] reward;

    public DailyMissionResponse(JObject data)
    {
        message = (string)data["message"];
        idDailyMission = (long)data["idDailyMission"];
        isSuccess = (bool)data["isSuccess"];
        reward = (byte[])data["reward"];
    }
}