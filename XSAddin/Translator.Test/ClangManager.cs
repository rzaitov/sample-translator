using System;
using System.IO;

namespace Translator.Test
{
	public class ClangManager : IDisposable
	{
		string tmpDir;
		public string TmpFolder {
			get {
				if (tmpDir == null) {
					tmpDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
					Directory.CreateDirectory (tmpDir);
				}

				return tmpDir;
			}
		}

		public ClangManager ()
		{
		}

		public string StoreFile (string content, string fileName)
		{
			string filePath = Path.Combine (TmpFolder, fileName);
			File.WriteAllText (filePath, content);

			return filePath;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			if (tmpDir != null)
				Directory.Delete (tmpDir);
		}

		#endregion
	}
}

