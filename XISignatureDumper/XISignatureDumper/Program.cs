using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
			DumpSignatures (map, "signatures.txt");
		}

		static void DumpSignatures (Dictionary<string, string> map, string path)
		{
			IEnumerable<string> lines = map.Select (kvp => string.Format ("{0}\n{1}", kvp.Key, kvp.Value));
			File.WriteAllLines (path, lines);
		}
	}
}
