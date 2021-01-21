using System.Collections.Generic;

namespace Modmail.Models
{
  public struct Attachment
  {
    public ulong ID;
    public ulong MessageID;
    public string Name;
    public ulong Sender;
    public string Source;
    public bool IsImage;
  }

  public struct Category
  {
    public string Name;
    public string Emoji;
    public ulong GuildID;
    public ulong ID;
    public bool IsActive;
    public ulong ChannelID;
  }

  public struct Edit
  {
    public string Content;
    public ulong MessageID;
    public int Version;
  }

  public struct Message
  {
    public string Content;
    public ulong ClientID;
    public List<Edit> Edits;
    public List<Attachment> Files;
    public bool IsDeleted;
    public ulong ModmailID;
    public ulong SenderID;
    public ulong ThreadID;
    public bool IsInternal;
  }

  public struct ModmailUser
  {
    public ulong ID;
  }

  public struct UserThread
  {
    public ulong AuthorID;
    public ulong ChannelID;
    public ulong ID;
    public bool IsActive;
    public List<Message> Messages;
    public ulong CategoryID;
  }

  public struct MuteStatus
  {
    public ulong UserID;
    public ulong Till;
    public ulong CategoryID;
    public string Reason;
    public bool IsIgnored;
  }


  public struct StandardReply
  {
    public string Reply;
    public string Name;
    public ulong ID;
  }

  public enum RoleLevel
  {
    Admin,
    Mod,
  }

  public struct Role
  {
    public ulong CategoryID;
    public ulong ID;
    public RoleLevel Level;
  }
}