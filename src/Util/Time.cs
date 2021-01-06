using System;

namespace Modmail.Util
{
  public class Time
  {
    /// <summary>
    /// <c>GetNow</c> gets the current time in since Unix epoch in
    /// milliseconds (used in the muting feature)
    /// </summary>
    /// <returns>Unix timestamp in milliseconds</returns>
    public static long GetNow()
    {
      return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
  }
}
