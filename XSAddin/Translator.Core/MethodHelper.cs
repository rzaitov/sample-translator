using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Translator.Core
{
	public static class MethodHelper
	{
		public static string ConvertToMehtodName (string selector)
		{
			string[] items = selector.Split (new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			string name = string.Join (string.Empty, items.Select (s => {
				var chars = s.ToCharArray ();
				chars [0] = char.ToUpper (chars [0]);
				return new string (chars);
			}));

			return name;
		}

		public static IEnumerable<string> Comment (string code)
		{
			var lines = code.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var l in lines)
				yield return string.Format ("// {0}\n", l.Trim ());
		}
	}
}

