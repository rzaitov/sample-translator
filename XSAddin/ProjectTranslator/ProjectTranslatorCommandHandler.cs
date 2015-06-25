using System;
using System.Collections.Generic;

using Gtk;
using MonoDevelop.Components.Commands;

using Translator.Parser;
using Translator.UI;

namespace Translator.Addin
{
	public class ProjectTranslatorCommandHandler : CommandHandler
	{
		Translator.UI.Settings window;
		XcodeProject xcodeProjectModel;
		ConversionPreferences preferences;

		protected override void Run ()
		{
			base.Run ();
			window = new Translator.UI.Settings ();
			window.OnRunAnalysisButtonPressed += HandleRunAnalysisButtonClick;
			window.OnRunGenerationButtonPressed += HandleRunGenerationButtonClick;
			window.Show ();
		}

		protected override void Update (CommandInfo info)
		{
			base.Update (info);
			info.Enabled = true;
		}

		List<Translator.Parser.Target> HandleRunAnalysisButtonClick (ConversionPreferences preferences)
		{
			this.preferences = preferences;
			xcodeProjectModel = XcodeProjectLoader.LoadProject (preferences.XcodeProjectPath);

			xcodeProjectModel.AnalyzeProjectDependecies ();
			xcodeProjectModel.AnalyzeProjectsFileStructure ();
			return xcodeProjectModel.Targets;
		}

		void HandleRunGenerationButtonClick (List<Translator.Parser.Target> targets)
		{
			xcodeProjectModel.Targets = targets;
			var solutionGenerator = new SolutionGenerator (xcodeProjectModel, preferences);
			solutionGenerator.Run ();
			window.Destroy ();
		}
	}
}

