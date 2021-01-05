using System;
using System.Collections.Generic;

namespace Modmail.Models
{
  public struct Attachment
  {
    UInt64 id;
    string name;
    UInt64 sender;
    string source;
    bool isImage;
  }

  public struct Category
  {
    string name;
    string emojiID;
    UInt64 guildID;
    UInt64 id;
    bool isActive;
    UInt64 channelID;
  }

  public struct DBCategory
  {
    string channelID;
    string emote;
    UInt64 guildID;
    UInt64 id;
    string name;
  }

  public struct CreateCategoryOpt
  {
    string name;
    string guildID;
    string emote;
    string channelID;
  }

  public struct Edit
  {
    string content;
    UInt64 message;
    UInt32 version;
  }

  public struct Message
  {
    string content;
    UInt64 clientID;
    List<Edit> edits;
    List<Attachment> files;
    bool isDeleted;
    UInt64 modmailID;
    UInt64 sender;
    UInt64 threadID;
    bool interna;
  }

  public struct DBMessage
  {
    string content;
    UInt64 clientID;
    bool isDeleted;
    UInt64 modmailID;
    UInt64 sender;
    UInt64 threadID;
    bool isInternal;
  }

  public struct ModmailUser
  {
    UInt64 id;
  }

  public struct Thread
  {
    ModmailUser author;
    UInt64 channel;
    UInt64 id;
    bool isActive;
    List<Message> messages;
    UInt16 category;
  }


  /**
   * @type MuteStatus
   * @property {UInt64} user
   * @property {number} till Unix Epoch in seconds
   * @property {CategoryID} category
   * @property {string} reason
   */
  public struct MuteStatus
  {
    UInt64 user;
    UInt64 till;
    UInt64 category;
    string reason;
  }


  public struct StandardReply
  {
    string reply;
    string name;
    string id;
  }

  public enum RoleLevel
  {
    Admin,
    Mod,
  }

  public struct Role
  {
    UInt64 category;
    UInt64 roleID;
    RoleLevel level;
  }
}