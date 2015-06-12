using System;

using Antlr4.Runtime;

namespace XcodeProjectParser
{
	public class LexerErrorListener : IAntlrErrorListener<int>
	{
		static LexerErrorListener errorHandler;

		public static LexerErrorListener ErrorHandler {
			get {
				if (errorHandler == null)
					errorHandler = new LexerErrorListener ();
				return errorHandler;
			}
		}

		public void SyntaxError (IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			Console.WriteLine (string.Format ("line {0}:{1} {2}", line, charPositionInLine, msg));
		}
	}
}
