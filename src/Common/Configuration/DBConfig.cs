using System;
using System.ComponentModel.DataAnnotations;

namespace Modmail.Configuration
{
  public class DBConfig
  {
    #nullable disable
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
  }
}
