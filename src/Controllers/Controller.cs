using log4net;

namespace Modmail.Controllers
{
  public class Controller
  {
    private string name;
    protected MainController main;

    public Controller(string name, MainController main)
    {
      this.name = name;
      this.main = main;
    }

    protected ILog GetLogger()
    {
      return LogManager.GetLogger($"(controller) {this.name}");
    }
  }
}