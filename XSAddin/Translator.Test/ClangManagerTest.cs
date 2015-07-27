using NUnit.Framework;
using System;
using System.IO;

namespace Translator.Test
{
	[TestFixture]
	public class ClangManagerTest
	{
		[Test]
		public void StoreContent ()
		{
			var manager = new ClangManager ();
			const string content = "Hello!";
			string filePath = manager.StoreFile (content, "tmp.m");

			Assert.AreEqual (content, File.ReadAllText (filePath));
		}
	}
}

