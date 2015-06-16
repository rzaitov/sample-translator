using System;
using System.Linq;

namespace Translator.Core
{
	public static class SelectorHelper
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
	}
}

