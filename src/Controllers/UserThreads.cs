using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Modmail.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using log4net;
using DSharpPlus.Exceptions;
using table = Modmail.Database.Tables;

namespace Modmail.Controllers
{
  public class UserThreads : Controller
  {
    private table.UserThreads db;
    public UserThreads(
      MainController main,
      table.UserThreads db
    ) : base("threads", main)
    {
      this.db = db;
    }

    /// <summary>
    /// <c>CreateThread</c> Create a brand new thread and channel.
    /// </summary>
    /// <param name="catID">The category to create the thread in</param>
    /// <param name="user">The user to create the thread for</param>
    /// <returns>A brand new thread</returns>
    /// <exception cref="ModmailException"></exception>
    public async Task<UserThread?> CreateThread(
      ulong catID,
      DiscordUser user
    )
    {
      ILog logger = GetLogger();
      DiscordChannel channel = await this.CreateChannel(catID, user);
      UserThread uThread = new UserThread
      {
        AuthorID = user.Id,
        CategoryID = catID,
        ChannelID = channel.Id,
        ID = 0,
        IsActive = true,
        Messages = new List<Message>(),
      };

      try
      {
        await this.db.Store(uThread);
        return uThread;
      } catch (Exception e)
      {
        logger.Error(e);
        throw new ModmailException(
          "An internal error has occurred, contact a server mod or administrator",
          e);
      }
    }

    /// <summary>
    /// <c>CreateChannel</c> Create a channel for a new thread
    /// </summary>
    /// <param name="catID">
    /// The category to create the thread channel in
    /// </param>
    /// <param name="user">The user to create the thread for</param>
    /// <returns>A nullable DiscordChannel</returns>
    /// <exception cref="ModmailException"></exception>
    private async Task<DiscordChannel> CreateChannel(
      ulong catID,
      DiscordUser user
    )
    {
      ILog logger = GetLogger();
      Categories cats = this.main.categories;
      DiscordChannel parent = await cats.GetParentChannel(catID);

      try
      {
        DiscordChannel channel = await parent.Guild.CreateChannelAsync(
          $"{user.Username}-{user.Discriminator}",
          ChannelType.Text,
          parent);

        return channel;
      } catch (UnauthorizedException e)
      {
        logger.Error(e);
        throw new ModmailException(
          "I don't have permissions to create a new thread channel.",
          e);
      }
    }
  }
}
