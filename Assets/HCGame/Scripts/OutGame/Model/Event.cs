using System;
using UnityEngine;

[System.Serializable]
public class Event
{
    public string Title { get; set; }

    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; }

    public float Prizepool { get; set; }

    public float EntryFee { get; set; }

    public int Players { get; set; }

    public string Description { get; set; }

    public string Rule { get; set; }

    public string LogoUrl { get; set; }

    public string BannerUrl { get; set; }

    
}
