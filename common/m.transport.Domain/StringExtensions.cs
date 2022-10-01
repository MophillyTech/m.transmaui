using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Utilities
{
	public static class StringExtensions
	{
		public static int? ParseInt(this string s)
		{
			int? retVal = null;
			int id;
			if (Int32.TryParse(s, out id))
			{
				retVal = id;
			}
			return retVal;
		}

		public static IEnumerable<string> SplitCorrectly(this string str, char ch)
		{
			if (str == null) return new string[0];
			return str.Split(new[] {ch}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
		}
	}
}
