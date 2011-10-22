using System.Globalization;
using System.Runtime.Versioning;

namespace System
{
  public class EnvironmentX
  {
    [ResourceExposure(ResourceScope.None)]
    internal static String GetResourceString(String key)
    {
      //return GetResourceFromDefault(key);
      return key;
    }

    [ResourceExposure(ResourceScope.None)]
    internal static String GetResourceString(String key, params Object[] values)
    {
      //String s = GetResourceFromDefault(key);
      string s = key;
      return String.Format(CultureInfo.CurrentCulture, s, values);
    }
  }
}
