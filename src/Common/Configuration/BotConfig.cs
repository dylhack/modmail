using System.ComponentModel.DataAnnotations;

namespace Modmail.Configuration
{
  public class BotConfig
  {
    [Required]
    public string token;
    [Required]
    public string prefix;
  }
}
