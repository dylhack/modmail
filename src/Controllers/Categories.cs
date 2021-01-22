using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using log4net;
using System;
using System.Threading.Tasks;
using Modmail.Models;
using table = Modmail.Database.Tables;

namespace Modmail.Controllers
{
  public class Categories : Controller
  {
    private table.Categories db;
    public Categories(
      MainController main,
      table.Categories db
    ) : base("categories", main)
    {
      this.db = db;
    }

    /// <summary>
    /// <c>GetParentChannel</c> Get the channel category for this category.
    /// </summary>
    /// <param name="catID"></param>
    /// <returns></returns>
    public async Task<DiscordChannel?> GetParentChannel(ulong catID)
    {
      ILog logger = GetLogger();
      Category? catOpt = await this.db.GetByID(catID);

      if (catOpt is null)
      {
        return null;
      }

      try
      {
        Category cat = catOpt.Value;
        DiscordClient client = this.main.GetClient();
        DiscordChannel chan = await client.GetChannelAsync(cat.ChannelID);
        return chan;
      } catch (NotFoundException)
      {
        return null;
      } catch (Exception e)
      {
        logger.Error(e);
      }
      return null;
    }
  }
}
