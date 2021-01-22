using System.Collections.Generic;
using System.Threading.Tasks;
using Modmail.Models;
using DSharpPlus;
using DSharpPlus.Entities;
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

    public async Task<UserThread?> CreateThread(
      ulong catID,
      DiscordUser user
    )
    {
      DiscordChannel? channel = await this.CreateChannel(user, catID);

      if (channel is null)
      {
        return null;
      }

      UserThread uThread = new UserThread
      {
        AuthorID = user.Id,
        CategoryID = catID,
        ChannelID = channel.Id,
        ID = 0,
        IsActive = true,
        Messages = new List<Message>(),
      };

      await this.db.Store(uThread);

      return uThread;
    }

    private async Task<DiscordChannel?> CreateChannel(
      DiscordUser user,
      ulong catID
    )
    {
      Categories cats = this.main.categories;
      DiscordChannel? parent = await cats.GetParentChannel(catID);

      if (parent is null)
      {
        return null;
      }
      DiscordChannel channel = await parent.Guild.CreateChannelAsync(
        $"{user.Username}-{user.Discriminator}",
        ChannelType.Text,
        parent);

      return channel;
    }
  }
}
