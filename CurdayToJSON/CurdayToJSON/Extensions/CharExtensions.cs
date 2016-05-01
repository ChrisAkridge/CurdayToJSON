using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurdayToJSON.Extensions
{
	public static class CharExtensions
	{
		public static bool IsOneOfChar(this char value, char[] checkChars)
		{
			return checkChars.Contains(value);
		}
	}
}
