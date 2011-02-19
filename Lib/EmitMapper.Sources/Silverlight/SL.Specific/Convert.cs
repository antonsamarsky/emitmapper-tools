using System.Globalization;

namespace System
{
  public static class ConvertSL
  {
    public static object ChangeType(object from, Type typeTo)
    {
      return Convert.ChangeType(from, typeTo, CultureInfo.CurrentUICulture);
    }
  }

  public static class EnumSL
  {
    public static object Parse(Type objectType, string str)
    {
      return Enum.Parse(objectType, str, true);
    }
  }
}
