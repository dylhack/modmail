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
    /// <param name="catID">Modmail Category ID</param>
    /// <returns>
    /// The category channel associated with the given category ID
    /// </returns>
    /// <exception cref="ModmailException">
    /// A human readable exception ready to be sent to a user on Discord. This
    /// exception accounts for all the other possible errors that might have
    /// occurred in this method.
    /// </exception>
    public async Task<DiscordChannel> GetParentChannel(ulong catID)
    {
      ILog logger = GetLogger();
      Category? catOpt = await this.db.GetByID(catID);

      if (catOpt is null)
      {
        throw new ModmailException($"The category \"{catID}\" doesn't exist.");
      }

      try
      {
        Category cat = catOpt.Value;
        DiscordClient client = this.main.GetClient();
        DiscordChannel chan = await client.GetChannelAsync(cat.ChannelID);
        return chan;
      } catch (NotFoundException e)
      {
        logger.Error(e);
        throw new ModmailException(
          "This category's parent channel is missing.",
          e);
      } catch (Exception e)
      {
        logger.Error(e);
        throw new ModmailException(
          "Something internal went wrong, please contact a mod or administrator.",
          e);
      }
    }
  }
}
