using System;

namespace EmitMapper
{
  public class ApplicationException : Exception
  {
    public ApplicationException()
    {
    }

    public ApplicationException(string message) : base(message)
    {
    }

    public ApplicationException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
