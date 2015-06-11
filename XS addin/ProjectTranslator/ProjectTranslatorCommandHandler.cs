using System;
using System.Collections.Generic;

using Gtk;
using MonoDevelop.Components.Commands;

using ProjectTranslatorUI;
using XcodeProjectParser;

namespace ProjectTranslator
{
	public class ProjectTranslatorCommandHandler : CommandHandler
	{
		XcodeProject xcodeProjectModel;
		ConversionPreferences preferences;

		protected override void Run ()
		{
			base.Run ();
			var window = new ProjectTranslatorUI.Settings ();
			window.OnRunAnalysisButtonPressed += HandleRunAnalysisButtonClick;
			window.OnRunGenerationButtonPressed += HandleRunGenerationButtonClick;
			window.Show ();
		}

		protected override void Update (CommandInfo info)
		{
			base.Update (info);
			info.Enabled = true;
		}

		List<XcodeProjectParser.Target> HandleRunAnalysisButtonClick (ConversionPreferences preferences)
		{
			this.preferences = preferences;
			xcodeProjectModel = XcodeProjectLoader.LoadProject (preferences.XcodeProjectPath);

			xcodeProjectModel.AnalyzeProjectDependecies ();
			xcodeProjectModel.AnalyzeProjectsFileStructure ();
			return xcodeProjectModel.Targets;
		}

		void HandleRunGenerationButtonClick (List<XcodeProjectParser.Target> targets)
		{
			xcodeProjectModel.Targets = targets;
			var solutionGenerator = new SolutionGenerator (xcodeProjectModel, preferences);
			solutionGenerator.Run ();
		}
	}
}

