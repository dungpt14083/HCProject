// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: TournamentRankingProto.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from TournamentRankingProto.proto</summary>
public static partial class TournamentRankingProtoReflection {

  #region Descriptor
  /// <summary>File descriptor for TournamentRankingProto.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static TournamentRankingProtoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChxUb3VybmFtZW50UmFua2luZ1Byb3RvLnByb3RvIqwBChZUb3VybmFtZW50",
          "UmFua2luZ1Byb3RvEhIKCnVzZXJDb2RlSWQYASABKAkSDgoGYXZhdGFyGAIg",
          "ASgJEhAKCHVzZXJuYW1lGAMgASgJEg4KBnJhdGluZxgEIAEoBRISCgpnb2xk",
          "UmV3YXJkGAUgASgDEhMKC3Rva2VuUmV3YXJkGAYgASgDEhQKDHRpY2tldFJl",
          "d2FyZBgHIAEoAxINCgVwb2ludBgIIAEoBSKqAQoSTGlzdFVzZXJUb3VybmFt",
          "ZW50Eg0KBWlzRW5kGAEgASgFEg8KB2lzQ2xhaW0YAiABKAUSEwoLZ3JvdXBS",
          "b29tSWQYAyABKAkSEQoJdG90YWxSb29tGAQgASgFEhcKD21pbmlHYW1lRXZl",
          "bnRJZBgFIAEoAxIzChJsaXN0VXNlclRvdXJuYW1lbnQYBiADKAsyFy5Ub3Vy",
          "bmFtZW50UmFua2luZ1Byb3RvYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::TournamentRankingProto), global::TournamentRankingProto.Parser, new[]{ "UserCodeId", "Avatar", "Username", "Rating", "GoldReward", "TokenReward", "TicketReward", "Point" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ListUserTournament), global::ListUserTournament.Parser, new[]{ "IsEnd", "IsClaim", "GroupRoomId", "TotalRoom", "MiniGameEventId", "ListUserTournament_" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class TournamentRankingProto : pb::IMessage<TournamentRankingProto>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<TournamentRankingProto> _parser = new pb::MessageParser<TournamentRankingProto>(() => new TournamentRankingProto());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<TournamentRankingProto> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::TournamentRankingProtoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public TournamentRankingProto() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public TournamentRankingProto(TournamentRankingProto other) : this() {
    userCodeId_ = other.userCodeId_;
    avatar_ = other.avatar_;
    username_ = other.username_;
    rating_ = other.rating_;
    goldReward_ = other.goldReward_;
    tokenReward_ = other.tokenReward_;
    ticketReward_ = other.ticketReward_;
    point_ = other.point_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public TournamentRankingProto Clone() {
    return new TournamentRankingProto(this);
  }

  /// <summary>Field number for the "userCodeId" field.</summary>
  public const int UserCodeIdFieldNumber = 1;
  private string userCodeId_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string UserCodeId {
    get { return userCodeId_; }
    set {
      userCodeId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "avatar" field.</summary>
  public const int AvatarFieldNumber = 2;
  private string avatar_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string Avatar {
    get { return avatar_; }
    set {
      avatar_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "username" field.</summary>
  public const int UsernameFieldNumber = 3;
  private string username_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string Username {
    get { return username_; }
    set {
      username_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "rating" field.</summary>
  public const int RatingFieldNumber = 4;
  private int rating_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int Rating {
    get { return rating_; }
    set {
      rating_ = value;
    }
  }

  /// <summary>Field number for the "goldReward" field.</summary>
  public const int GoldRewardFieldNumber = 5;
  private long goldReward_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long GoldReward {
    get { return goldReward_; }
    set {
      goldReward_ = value;
    }
  }

  /// <summary>Field number for the "tokenReward" field.</summary>
  public const int TokenRewardFieldNumber = 6;
  private long tokenReward_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long TokenReward {
    get { return tokenReward_; }
    set {
      tokenReward_ = value;
    }
  }

  /// <summary>Field number for the "ticketReward" field.</summary>
  public const int TicketRewardFieldNumber = 7;
  private long ticketReward_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long TicketReward {
    get { return ticketReward_; }
    set {
      ticketReward_ = value;
    }
  }

  /// <summary>Field number for the "point" field.</summary>
  public const int PointFieldNumber = 8;
  private int point_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int Point {
    get { return point_; }
    set {
      point_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as TournamentRankingProto);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(TournamentRankingProto other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (UserCodeId != other.UserCodeId) return false;
    if (Avatar != other.Avatar) return false;
    if (Username != other.Username) return false;
    if (Rating != other.Rating) return false;
    if (GoldReward != other.GoldReward) return false;
    if (TokenReward != other.TokenReward) return false;
    if (TicketReward != other.TicketReward) return false;
    if (Point != other.Point) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (UserCodeId.Length != 0) hash ^= UserCodeId.GetHashCode();
    if (Avatar.Length != 0) hash ^= Avatar.GetHashCode();
    if (Username.Length != 0) hash ^= Username.GetHashCode();
    if (Rating != 0) hash ^= Rating.GetHashCode();
    if (GoldReward != 0L) hash ^= GoldReward.GetHashCode();
    if (TokenReward != 0L) hash ^= TokenReward.GetHashCode();
    if (TicketReward != 0L) hash ^= TicketReward.GetHashCode();
    if (Point != 0) hash ^= Point.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (UserCodeId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(UserCodeId);
    }
    if (Avatar.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Avatar);
    }
    if (Username.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(Username);
    }
    if (Rating != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(Rating);
    }
    if (GoldReward != 0L) {
      output.WriteRawTag(40);
      output.WriteInt64(GoldReward);
    }
    if (TokenReward != 0L) {
      output.WriteRawTag(48);
      output.WriteInt64(TokenReward);
    }
    if (TicketReward != 0L) {
      output.WriteRawTag(56);
      output.WriteInt64(TicketReward);
    }
    if (Point != 0) {
      output.WriteRawTag(64);
      output.WriteInt32(Point);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (UserCodeId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(UserCodeId);
    }
    if (Avatar.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Avatar);
    }
    if (Username.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(Username);
    }
    if (Rating != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(Rating);
    }
    if (GoldReward != 0L) {
      output.WriteRawTag(40);
      output.WriteInt64(GoldReward);
    }
    if (TokenReward != 0L) {
      output.WriteRawTag(48);
      output.WriteInt64(TokenReward);
    }
    if (TicketReward != 0L) {
      output.WriteRawTag(56);
      output.WriteInt64(TicketReward);
    }
    if (Point != 0) {
      output.WriteRawTag(64);
      output.WriteInt32(Point);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (UserCodeId.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(UserCodeId);
    }
    if (Avatar.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Avatar);
    }
    if (Username.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Username);
    }
    if (Rating != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Rating);
    }
    if (GoldReward != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(GoldReward);
    }
    if (TokenReward != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(TokenReward);
    }
    if (TicketReward != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(TicketReward);
    }
    if (Point != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Point);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(TournamentRankingProto other) {
    if (other == null) {
      return;
    }
    if (other.UserCodeId.Length != 0) {
      UserCodeId = other.UserCodeId;
    }
    if (other.Avatar.Length != 0) {
      Avatar = other.Avatar;
    }
    if (other.Username.Length != 0) {
      Username = other.Username;
    }
    if (other.Rating != 0) {
      Rating = other.Rating;
    }
    if (other.GoldReward != 0L) {
      GoldReward = other.GoldReward;
    }
    if (other.TokenReward != 0L) {
      TokenReward = other.TokenReward;
    }
    if (other.TicketReward != 0L) {
      TicketReward = other.TicketReward;
    }
    if (other.Point != 0) {
      Point = other.Point;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          UserCodeId = input.ReadString();
          break;
        }
        case 18: {
          Avatar = input.ReadString();
          break;
        }
        case 26: {
          Username = input.ReadString();
          break;
        }
        case 32: {
          Rating = input.ReadInt32();
          break;
        }
        case 40: {
          GoldReward = input.ReadInt64();
          break;
        }
        case 48: {
          TokenReward = input.ReadInt64();
          break;
        }
        case 56: {
          TicketReward = input.ReadInt64();
          break;
        }
        case 64: {
          Point = input.ReadInt32();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          UserCodeId = input.ReadString();
          break;
        }
        case 18: {
          Avatar = input.ReadString();
          break;
        }
        case 26: {
          Username = input.ReadString();
          break;
        }
        case 32: {
          Rating = input.ReadInt32();
          break;
        }
        case 40: {
          GoldReward = input.ReadInt64();
          break;
        }
        case 48: {
          TokenReward = input.ReadInt64();
          break;
        }
        case 56: {
          TicketReward = input.ReadInt64();
          break;
        }
        case 64: {
          Point = input.ReadInt32();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class ListUserTournament : pb::IMessage<ListUserTournament>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ListUserTournament> _parser = new pb::MessageParser<ListUserTournament>(() => new ListUserTournament());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<ListUserTournament> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::TournamentRankingProtoReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public ListUserTournament() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public ListUserTournament(ListUserTournament other) : this() {
    isEnd_ = other.isEnd_;
    isClaim_ = other.isClaim_;
    groupRoomId_ = other.groupRoomId_;
    totalRoom_ = other.totalRoom_;
    miniGameEventId_ = other.miniGameEventId_;
    listUserTournament_ = other.listUserTournament_.Clone();
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public ListUserTournament Clone() {
    return new ListUserTournament(this);
  }

  /// <summary>Field number for the "isEnd" field.</summary>
  public const int IsEndFieldNumber = 1;
  private int isEnd_;
  /// <summary>
  /// 1 - tournament đã kết thúc, 2 - tournament vẫn đang diễn ra
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int IsEnd {
    get { return isEnd_; }
    set {
      isEnd_ = value;
    }
  }

  /// <summary>Field number for the "isClaim" field.</summary>
  public const int IsClaimFieldNumber = 2;
  private int isClaim_;
  /// <summary>
  /// 1 - được claim , 2 - không
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int IsClaim {
    get { return isClaim_; }
    set {
      isClaim_ = value;
    }
  }

  /// <summary>Field number for the "groupRoomId" field.</summary>
  public const int GroupRoomIdFieldNumber = 3;
  private string groupRoomId_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string GroupRoomId {
    get { return groupRoomId_; }
    set {
      groupRoomId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "totalRoom" field.</summary>
  public const int TotalRoomFieldNumber = 4;
  private int totalRoom_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int TotalRoom {
    get { return totalRoom_; }
    set {
      totalRoom_ = value;
    }
  }

  /// <summary>Field number for the "miniGameEventId" field.</summary>
  public const int MiniGameEventIdFieldNumber = 5;
  private long miniGameEventId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long MiniGameEventId {
    get { return miniGameEventId_; }
    set {
      miniGameEventId_ = value;
    }
  }

  /// <summary>Field number for the "listUserTournament" field.</summary>
  public const int ListUserTournament_FieldNumber = 6;
  private static readonly pb::FieldCodec<global::TournamentRankingProto> _repeated_listUserTournament_codec
      = pb::FieldCodec.ForMessage(50, global::TournamentRankingProto.Parser);
  private readonly pbc::RepeatedField<global::TournamentRankingProto> listUserTournament_ = new pbc::RepeatedField<global::TournamentRankingProto>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public pbc::RepeatedField<global::TournamentRankingProto> ListUserTournament_ {
    get { return listUserTournament_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as ListUserTournament);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(ListUserTournament other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (IsEnd != other.IsEnd) return false;
    if (IsClaim != other.IsClaim) return false;
    if (GroupRoomId != other.GroupRoomId) return false;
    if (TotalRoom != other.TotalRoom) return false;
    if (MiniGameEventId != other.MiniGameEventId) return false;
    if(!listUserTournament_.Equals(other.listUserTournament_)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (IsEnd != 0) hash ^= IsEnd.GetHashCode();
    if (IsClaim != 0) hash ^= IsClaim.GetHashCode();
    if (GroupRoomId.Length != 0) hash ^= GroupRoomId.GetHashCode();
    if (TotalRoom != 0) hash ^= TotalRoom.GetHashCode();
    if (MiniGameEventId != 0L) hash ^= MiniGameEventId.GetHashCode();
    hash ^= listUserTournament_.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (IsEnd != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(IsEnd);
    }
    if (IsClaim != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(IsClaim);
    }
    if (GroupRoomId.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(GroupRoomId);
    }
    if (TotalRoom != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(TotalRoom);
    }
    if (MiniGameEventId != 0L) {
      output.WriteRawTag(40);
      output.WriteInt64(MiniGameEventId);
    }
    listUserTournament_.WriteTo(output, _repeated_listUserTournament_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (IsEnd != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(IsEnd);
    }
    if (IsClaim != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(IsClaim);
    }
    if (GroupRoomId.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(GroupRoomId);
    }
    if (TotalRoom != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(TotalRoom);
    }
    if (MiniGameEventId != 0L) {
      output.WriteRawTag(40);
      output.WriteInt64(MiniGameEventId);
    }
    listUserTournament_.WriteTo(ref output, _repeated_listUserTournament_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (IsEnd != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(IsEnd);
    }
    if (IsClaim != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(IsClaim);
    }
    if (GroupRoomId.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(GroupRoomId);
    }
    if (TotalRoom != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(TotalRoom);
    }
    if (MiniGameEventId != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(MiniGameEventId);
    }
    size += listUserTournament_.CalculateSize(_repeated_listUserTournament_codec);
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(ListUserTournament other) {
    if (other == null) {
      return;
    }
    if (other.IsEnd != 0) {
      IsEnd = other.IsEnd;
    }
    if (other.IsClaim != 0) {
      IsClaim = other.IsClaim;
    }
    if (other.GroupRoomId.Length != 0) {
      GroupRoomId = other.GroupRoomId;
    }
    if (other.TotalRoom != 0) {
      TotalRoom = other.TotalRoom;
    }
    if (other.MiniGameEventId != 0L) {
      MiniGameEventId = other.MiniGameEventId;
    }
    listUserTournament_.Add(other.listUserTournament_);
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          IsEnd = input.ReadInt32();
          break;
        }
        case 16: {
          IsClaim = input.ReadInt32();
          break;
        }
        case 26: {
          GroupRoomId = input.ReadString();
          break;
        }
        case 32: {
          TotalRoom = input.ReadInt32();
          break;
        }
        case 40: {
          MiniGameEventId = input.ReadInt64();
          break;
        }
        case 50: {
          listUserTournament_.AddEntriesFrom(input, _repeated_listUserTournament_codec);
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          IsEnd = input.ReadInt32();
          break;
        }
        case 16: {
          IsClaim = input.ReadInt32();
          break;
        }
        case 26: {
          GroupRoomId = input.ReadString();
          break;
        }
        case 32: {
          TotalRoom = input.ReadInt32();
          break;
        }
        case 40: {
          MiniGameEventId = input.ReadInt64();
          break;
        }
        case 50: {
          listUserTournament_.AddEntriesFrom(ref input, _repeated_listUserTournament_codec);
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code
