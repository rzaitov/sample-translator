using System;
using System.Net.Http;
using System.Threading.Tasks;

using ModernHttpClient;
using MarkdownLog;
using MarkdownDeep;
using System.IO;

namespace Translator.Logger
{
	public sealed class Logger
	{
		static readonly Logger instance = new Logger ();

		MarkdownContainer logContainer;

		static Logger ()
		{
		}

		public static Logger Default {
			get {
				return instance;
			}
		}

		Logger ()
		{
			logContainer = new MarkdownContainer ();
		}

		public void WriteHeader (string headerText)
		{
			if (string.IsNullOrEmpty (headerText))
				throw new ArgumentException ("headerText");
			
			logContainer.Append (headerText.ToMarkdownHeader ());
		}

		public void WriteSubHeader (string subHeaderText)
		{
			if (string.IsNullOrEmpty (subHeaderText))
				throw new ArgumentException ("subHeaderText");
			
			logContainer.Append (subHeaderText.ToMarkdownSubHeader ());
		}

		public void WriteMessage (string message)
		{
			if (string.IsNullOrEmpty (message))
				throw new ArgumentException ("message");

			logContainer.Append (message.ToMarkdownParagraph ());
		}

		public void WriteNumberedList (string[] listItems)
		{
			if (listItems == null || listItems.Length == 0)
				throw new ArgumentException ("listItems");

			logContainer.Append (listItems.ToMarkdownNumberedList ());
		}

		public void WriteBulletedList (string[] listItems)
		{
			if (listItems == null || listItems.Length == 0)
				throw new ArgumentException ("listItems");

			logContainer.Append (listItems.ToMarkdownBulletedList ());
		}

		public void WriteTable (object[] tableItems)
		{
			if (tableItems == null || tableItems.Length == 0)
				throw new ArgumentException ("tableItems");

			logContainer.Append (tableItems.ToMarkdownTable ());
		}

		public void WriteHorizontalDevider ()
		{
			logContainer.Append (new HorizontalRule ());
		}

		public void SaveLog (string path)
		{
//			File.AppendAllText ("date.txt", );
		}
	}
}

