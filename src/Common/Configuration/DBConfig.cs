using System;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;

namespace Modmail.Configuration
{
  public class DBConfig
  {
    [Required]
    public string address;
    [Required]
    public UInt16 port;
    [Required]
    public string username;
    [Required]
    public string password;
    [Required]
    public string database;
    [Required]
    public string schema;
  }
}
