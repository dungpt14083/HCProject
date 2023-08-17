using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtils 
{
    public static string FormatMoneyK(double value, int digit = 1)
    {
        //if (value < 0) value *= -1;
        if(value >= 1000000000000)
        {
            value /= 1000000000000;
            return Math.Round(value,digit) + "T";
        }

        if (value >= 1000000000)
        {
            value /= 1000000000;
            return Math.Round(value, digit) + "B";
        }

        if (value >= 1000000)
        {
            value /= 1000000;
            return Math.Round(value, digit) + "M";
        }
        
        if (!(value > 9999)) return value.ToString();
        value /= 1000;
        return Math.Round(value, digit) + "K";
    }


    //Convert number to stt
    public static string ConvertNumberToStt(int value)
    {
        string ordinal = value.ToString("D1");
        switch (value)
        {
            case 1: ordinal += "st"; break;
            case 2: ordinal += "nd"; break;
            case 3: ordinal += "rd"; break;
            default: ordinal += "th"; break;
        }
        return ordinal;
    }

    //Convert number to stt
    public static string ConvertRewardTypeToString(RewardType type)
    {
        string ordinal = "";
        switch (type)
        {
            case RewardType.Ticket: ordinal += "Ticket"; break;
            case RewardType.Token: ordinal += "Token"; break;
            case RewardType.Gold: ordinal += "Gold"; break;
            default: ordinal += ""; break;
        }
        return ordinal;
    }
   
   
}
