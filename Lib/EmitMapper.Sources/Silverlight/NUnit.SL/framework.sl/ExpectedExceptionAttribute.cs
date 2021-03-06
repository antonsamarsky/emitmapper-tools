// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

namespace NUnit.Framework
{
	using System;

	/// <summary>
	/// Enumeration indicating how the expected message parameter is to be used
	/// </summary>
	public enum MessageMatch
	{
		/// Expect an exact match
		Exact,	
		/// Expect a message containing the parameter string
		Contains,
		/// Match the regular expression provided as a parameter
		Regex,
        /// Expect a message that starts with the parameter string
        StartsWith
	}

	/// <summary>
	/// ExpectedExceptionAttribute
	/// </summary>
	/// 
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class ExpectedExceptionAttribute : Attribute
	{
		private Type expectedException;
		private string expectedExceptionName;
		private string expectedMessage;
		private MessageMatch matchType;
		private string userMessage;
		private string handler;

		/// <summary>
		/// Constructor for a non-specific exception
		/// </summary>
		public ExpectedExceptionAttribute()
		{
		}

		/// <summary>
		/// Constructor for a given type of exception
		/// </summary>
		/// <param name="exceptionType">The type of the expected exception</param>
		public ExpectedExceptionAttribute(Type exceptionType)
		{
			this.expectedException = exceptionType;
			this.expectedExceptionName = exceptionType.FullName;
		}

		/// <summary>
		/// Constructor for a given exception name
		/// </summary>
		/// <param name="exceptionName">The full name of the expected exception</param>
		public ExpectedExceptionAttribute(string exceptionName)
		{
			this.expectedExceptionName = exceptionName;
		}

		/// <summary>
		/// Gets or sets the expected exception type
		/// </summary>
		public Type ExpectedException
		{
			get{ return expectedException; }
			set
            { 
                expectedException = value;
                expectedExceptionName = expectedException.FullName;
            }
		}

		/// <summary>
		/// Gets or sets the full Type name of the expected exception
		/// </summary>
		public string ExpectedExceptionName
		{
			get{ return expectedExceptionName; }
			set{ expectedExceptionName = value; }
		}

		/// <summary>
		/// Gets or sets the expected message text
		/// </summary>
		public string ExpectedMessage 
		{
			get { return expectedMessage; }
			set { expectedMessage = value; }
		}

		/// <summary>
		/// Gets or sets the user message displayed in case of failure
		/// </summary>
		public string UserMessage
		{
			get { return userMessage; }
			set { userMessage = value; }
		}

		/// <summary>
		///  Gets or sets the type of match to be performed on the expected message
		/// </summary>
		public MessageMatch MatchType
		{
			get { return matchType; }
			set { matchType = value; }
		}

		/// <summary>
		///  Gets the name of a method to be used as an exception handler
		/// </summary>
		public string Handler
		{
			get { return handler; }
			set { handler = value; }
		}
	}
}
