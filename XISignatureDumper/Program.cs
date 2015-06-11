using System;
using System.Collections.Generic;

namespace XISignatureDumper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			var locator = new AssemblyPathLocator ();
			var extruder = new SignatureExtruder (locator.GetAssemblyPath (Platform.iOS));

			Dictionary<string, string> map = extruder.CollectSignatures ();
			foreach (var kvp in map) {
				Console.WriteLine (kvp.Key);
				Console.WriteLine (kvp.Value);
				Console.WriteLine ();
			}
		}
	}
}
