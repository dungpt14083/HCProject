//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class UserInfo
//{
//    public long id;
//    public long gold;
//    public string userCodeId;
//    public string userName;
//    public string avatar;
//    public long token;
//    public long ticket;
//    public int level;
//    public int numberInvited;
//    public int maxNumberInvited;
//    public long exp;
//    public long expToNextLevel;
//    public string referralCode;
//    public List<MmrUser> mmr;
//    public UserInfo()
//    {

//    }
//    public UserInfo(long _id, string _userCodeId, string _userName, string _avatar, long _gold, long _token, long _ticket, int _level, int _numberInvited)
//    {
//        this.id = _id;
//        this.userCodeId = _userCodeId;
//        this.userName = _userName;
//        this.avatar = _avatar;
//        this.gold = _gold;
//        this.token = _token;
//        this.ticket = _ticket;
//        this.level = _level;
//        this.numberInvited = _numberInvited;
//    }
//    public UserInfo(UserInfo userInfo)
//    {
//        this.id = userInfo.id;
//        this.userCodeId = userInfo.userCodeId;
//        this.userName = userInfo.userName;
//        this.avatar = userInfo.avatar;
//        this.gold = userInfo.gold;
//        this.token = userInfo.token;
//        this.ticket = userInfo.ticket;
//        this.level = userInfo.level;
//        this.numberInvited = userInfo.numberInvited;
//    }
//    public UserInfo(UserDataProto userInfo)
//    {
//        this.id = userInfo.Id;
//        this.userName = userInfo.UserName;
//        this.gold = userInfo.UserGold;
//        this.token = userInfo.UserToken;
//        this.ticket = userInfo.UserTicket;
//        this.userCodeId = userInfo.UserCodeId;
//        //this.avatar = userInfo.UserAvatar;
//        this.level = userInfo.Level;
//        this.numberInvited = userInfo.NumberInvited;
//        this.maxNumberInvited = userInfo.MaxNumberInvited;
//        this.exp = userInfo.Exp;
//        this.expToNextLevel = userInfo.ExpToNextLevel;
//        this.referralCode = userInfo.ReferralCode;
//        this.mmr = userInfo.Mmr.ToList();
//    }

    
//}
