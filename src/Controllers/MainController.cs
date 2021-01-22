using DSharpPlus;
using Modmail.Database;

namespace Modmail.Controllers
{
  public class MainController
  {
    public Categories categories;
    public UserThreads threads;
    private DiscordClient client;
    public MainController(
      DiscordClient client,
      DBManager db
    )
    {
      this.client = client;
      this.threads = new UserThreads(this, db.threads);
      this.categories = new Categories(this, db.categories);
    }

    public DiscordClient GetClient()
    {
      return this.client;
    }
  }
}
