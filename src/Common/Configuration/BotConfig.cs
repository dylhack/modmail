using System.ComponentModel.DataAnnotations;

namespace Modmail.Configuration
{
  public class BotConfig
  {
    #nullable disable
    [Required]
    public string token;
    [Required]
    public string prefix;
  }
}
