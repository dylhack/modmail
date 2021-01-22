using System;

namespace Modmail.Models
{
  public class ModmailException : Exception
  {
    public ModmailException(string message) : base(message)
    { }

    public ModmailException(string message, Exception inner) : base(message, inner)
    { }
  }
}
