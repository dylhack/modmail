using System.Collections.Generic;

namespace Modmail.Models
{
  public struct Attachment
  {
    public long ID;
    public long MessageID;
    public string Name;
    public long Sender;
    public string Source;
    public bool IsImage;
  }

  public struct Category
  {
    public string Name;
    public string Emoji;
    public long GuildID;
    public long ID;
    public bool IsActive;
    public long ChannelID;
  }

  public struct Edit
  {
    public string Content;
    public long MessageID;
    public uint Version;
  }

  public struct Message
  {
    public string Content;
    public long ClientID;
    public List<Edit> Edits;
    public List<Attachment> files;
    public bool IsDeleted;
    public long ModmailID;
    public long Sender;
    public long ThreadID;
    public bool IsInternal;
  }

  public struct ModmailUser
  {
    public long ID;
  }

  public struct Thread
  {
    public ModmailUser Author;
    public long Channel;
    public long ID;
    public bool IsActive;
    public List<Message> Messages;
    public long CategoryID;
  }

  public struct MuteStatus
  {
    public long UserID;
    public long Till;
    public long CategoryID;
    public string Reason;
  }


  public struct StandardReply
  {
    public string Reply;
    public string Name;
    public string ID;
  }

  public enum RoleLevel
  {
    Admin,
    Mod,
  }

  public struct Role
  {
    public long CategoryID;
    public long RoleID;
    public RoleLevel Level;
  }
}