using System;

[Serializable]
public class UserData_Tutorial
{
    public bool isFinished;
    public int currentTutorialId;
}

public class UserDataResponse
{
    public long userId;
    public string userName;
    public int userRole;
    public string token;
    public bool isCheckIn;
    public UserData_Tutorial tutorial;
}