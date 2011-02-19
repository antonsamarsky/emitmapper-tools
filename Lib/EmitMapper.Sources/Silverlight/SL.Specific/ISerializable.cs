// Type: System.Runtime.Serialization.ISerializable
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
  [ComVisible(true)]
  public interface ISerializable
  {
    void GetObjectData(SerializationInfo info, StreamingContext context);
  }
}