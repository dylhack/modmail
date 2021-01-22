using YamlDotNet.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Modmail.Configuration
{
  public class Config
  {
    #nullable disable
    [Required]
    public DBConfig database;
    [Required]
    public BotConfig bot;

    public override string ToString()
    {
      ISerializer se = new SerializerBuilder()
        .Build();

      return se.Serialize(this);
    }
  }
}