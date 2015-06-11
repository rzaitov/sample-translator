﻿using System;

namespace XISignatureDumper
{
	public class AssemblyPathLocator
	{
		const string  XILibPath = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/Xamarin.iOS.dll";
		const string  XMLibPath = "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/i386/full/Xamarin.Mac.dll";

		public AssemblyPathLocator ()
		{
		}

		public string GetAssemblyPath (Platform platform)
		{
			switch (platform) {
				case Platform.iOS:
					return XILibPath;

				case Platform.Mac:
					return XMLibPath;

				default:
					throw new NotImplementedException ();
			}
		}
	}
}

