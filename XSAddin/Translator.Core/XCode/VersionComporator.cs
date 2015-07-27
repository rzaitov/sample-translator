using System;
using System.Collections.Generic;

namespace Translator.Core
{
	public class VersionComporator : IComparer<VersionInfo>
	{
		public int Compare (VersionInfo x, VersionInfo y)
		{
			int comparision = x.Major - y.Major;
			if (comparision != 0)
				return comparision;

			return x.Minor - y.Minor;
		}
	}
}