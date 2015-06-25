using System;
using System.IO;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Translator.Parser
{
	public class XcodeProjectReader
	{
		public static XcodeProject Parse (string projectDump)
		{
			var input = new AntlrInputStream (projectDump);
			var lexer = new XcodeProjectLexer (input);
			lexer.RemoveErrorListeners ();
			lexer.AddErrorListener (LexerErrorListener.ErrorHandler);

			var tokens = new CommonTokenStream (lexer);
			var parser = new XcodeProjectParser (tokens) {
				BuildParseTree = true
			};

			parser.RemoveErrorListeners ();
			parser.AddErrorListener (ParsingErrorListener.ErrorHandler);
			IParseTree tree = parser.project ();

			var visitor = new ParseTreeWalker ();
			var parseListener = new ProjectParserListener ();
			visitor.Walk (parseListener, tree);

			return parseListener.ProjectModel;
		}
	}
}
