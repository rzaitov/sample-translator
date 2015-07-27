using System;

namespace Translator.Core
{
	// half open range [min - max)
	public class XCodeCriteria
	{
		public VersionInfo Min { get; set; }
		public VersionInfo Max { get; set; }

		public bool IsSatisfy(VersionInfo version)
		{
			return IsMinSatisfy (version) && IsMaxSutisfy (version);
		}

		public bool IsMinSatisfy (VersionInfo version)
		{
			if (Min == null)
				return true;

			return version.Major >= Min.Major || version.Major == Min.Major && version.Minor >= Min.Minor;
		}

		public bool IsMaxSutisfy (VersionInfo version)
		{
			if (Max == null)
				return true;

			return version.Major < Max.Major || version.Major == Max.Major && version.Minor < Max.Minor;
		}
	}
}