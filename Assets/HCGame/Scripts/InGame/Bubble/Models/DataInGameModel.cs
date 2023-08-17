using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string uId { get; set; }
    public string username { get; set; }
    public int avatar { get; set; }
    public int frame { get; set; }
    public int point { get; set; }
    public int tigaCoin { get; set; }
    public int lifes { get; set; }
    public bool isFinishTutorial { get; set; }
    public int level { get; set; }
    public DateTime timeLoginFirst { get; set; }
    public int bgId { get; set; }
    public int ballId { get; set; }
}
public class DataGamePlay
{
    public int scores { get; set; }
    public int itemBomb { get; set; }
    public int itemRocket { get; set; }
    public int itemColorful { get; set; }
    public int moves { get; set; }
}

public class ScoreCompute
{
    public int score { get; set; }
    public int brokenBubbleOnCluster { get; set; }
    public int brokenBubbleOnHole100 { get; set; }
    public int brokenBubbleOnHole200 { get; set; }
}
public class CompetitorInfo
{
    public string name { get; set; }
    public int score { get; set; }
}

public class RoomInfo
{
    public int roomID { get; set; }
    public int CompetitorName { get; set; }
    public int CompetitorAvatarUrl { get; set; }
}
public class Competitor
{
    public int score { get; set; }
}