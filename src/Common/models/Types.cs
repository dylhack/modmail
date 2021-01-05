using System;
using System.Collections.Generic;

namespace Modmail.Models
{
  public struct Attachment
  {
    public UInt64 ID;
    public UInt64 MessageID;
    public string Name;
    public UInt64 Sender;
    public string Source;
    public bool IsImage;
  }

  public struct Category
  {
    public string Name;
    public string Emoji;
    public UInt64 GuildID;
    public UInt64 ID;
    public bool IsActive;
    public UInt64 ChannelID;
  }

  public struct Edit
  {
    public string Content;
    public UInt64 MessageID;
    public UInt32 Version;
  }

  public struct Message
  {
    public string Content;
    public UInt64 ClientID;
    public List<Edit> Edits;
    public List<Attachment> files;
    public bool IsDeleted;
    public UInt64 ModmailID;
    public UInt64 Sender;
    public UInt64 ThreadID;
    public bool IsInternal;
  }

  public struct ModmailUser
  {
    public UInt64 ID;
  }

  public struct Thread
  {
    public ModmailUser Author;
    public UInt64 Channel;
    public UInt64 ID;
    public bool IsActive;
    public List<Message> Messages;
    public UInt64 CategoryID;
  }

  public struct MuteStatus
  {
    public UInt64 UserID;
    public UInt64 Till;
    public UInt64 CategoryID;
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
    public UInt64 CategoryID;
    public UInt64 RoleID;
    public RoleLevel Level;
  }
}