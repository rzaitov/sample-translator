using System;

using Antlr4.Runtime;

namespace Translator.Parser
{
	public class ParsingErrorListener : IAntlrErrorListener<IToken>
	{
		static ParsingErrorListener errorHandler;

		public static ParsingErrorListener ErrorHandler {
			get {
				if (errorHandler == null)
					errorHandler = new ParsingErrorListener ();
				return errorHandler;
			}
		}

		public void SyntaxError (IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			Console.WriteLine (string.Format ("token text: {0} at line {1}:{2} Message: {3}", offendingSymbol.Text, line, charPositionInLine, msg));
		}
	}
}
