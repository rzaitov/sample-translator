using System.Collections.Generic;

namespace Translator.Addin
{
	public class CodeTranslationConfiguration
	{
		string projectNamespace;
		public string ProjectNamespace {
			get {
				return projectNamespace;
			}
			set {
				projectNamespace = value.Replace (" ", string.Empty);
			}
		}

		public string ProjectPath { get; set; }

		public string PCHFilePath { get; set; }

		public IList<string> SourceFilePaths { get; set; }

		public IList<string> HeaderFilePaths { get; set; }

		public IList<string> Frameworks { get; set; }

		public List<string> HeaderSearchPaths { get; set; }

		public CodeTranslationConfiguration ()
		{
			SourceFilePaths = new List<string> ();
			Frameworks = new List<string> ();
			HeaderFilePaths = new List<string> ();
		}
	}
}

