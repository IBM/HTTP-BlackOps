using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Exception thrown by the diff algorithm and its implementations
	/// </summary>
	public class DiffException : Exception
	{

		public DiffException(string message):base(message)
		{ 
					
		}
	}
}
