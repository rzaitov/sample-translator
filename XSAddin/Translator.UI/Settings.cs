using System;
using Gtk;
using System.Collections.Generic;
using Translator.Parser;

namespace Translator.UI
{
	enum ActivePhase
	{
		None = 0,
		AnalyzeXcodeProject,
		GenerateCSharpProject
	}

	public partial class Settings : Window
	{
		List<Translator.Parser.Target> targets;
		List<Widget> analysisPhaseControls;
		List<Widget> generationPhaseControls;
		FileChooserDialog fileChooser;

		public Func<ConversionPreferences, List<Translator.Parser.Target>> OnRunAnalysisButtonPressed;
		public Action<List<Translator.Parser.Target>> OnRunGenerationButtonPressed;

		public Settings () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			analysisPhaseControls = new List<Widget> {
				inputProjectLabel,
				inputProjectEntry,
				outputProjectLabel,
				outputProjectFolderEntry,
				analyzeXcodeProjectButton,
				overwriteAppIcons,
				overwriteLaunchImages,
				selectButton,
				selectOuputFolderButton
			};

			generationPhaseControls = new List<Widget> {
				setProjectTypeLabel,
				setProjectTypeCombobox,
				runButton
			};

			SetActivePhase (ActivePhase.AnalyzeXcodeProject);
		}

		protected void SelectInputProject (object sender, EventArgs e)
		{
			var fileFilter = new FileFilter () {
				Name = "*.xcodeproj"
			};

			fileFilter.AddPattern ("*.xcodeproj");

			fileChooser = new FileChooserDialog ("Select Xcode project", this, FileChooserAction.SelectFolder, new object[] {
				Stock.Cancel, ResponseType.Cancel, Stock.Ok, ResponseType.Ok
			}) {
				SelectMultiple = false
			};

			fileChooser.AddFilter (fileFilter);

			var response = (ResponseType)fileChooser.Run ();

			if (response == ResponseType.Ok)
				inputProjectEntry.Text = System.IO.Path.Combine (fileChooser.CurrentFolderUri, fileChooser.Filename);

			fileChooser.Destroy ();
		}

		protected void RunUtility (object sender, EventArgs e)
		{
			if (OnRunGenerationButtonPressed != null)
				OnRunGenerationButtonPressed (targets);
		}

		void ShowAlertDialog (string message)
		{
			var alert = new MessageDialog (
				this,
	            DialogFlags.Modal,
	            MessageType.Error,
	            ButtonsType.Close,
	            message
            );

			alert.Title = "Error";

			alert.Run ();
			alert.Destroy ();
		}

		protected void SelectSharpSolutionFolder (object sender, EventArgs e)
		{

			fileChooser = new FileChooserDialog ("Select C# project output folder", this, FileChooserAction.SelectFolder, new object[] {
				Stock.Cancel, ResponseType.Cancel, Stock.Ok, ResponseType.Ok
			}) {
				SelectMultiple = false
			};

			var response = (ResponseType)fileChooser.Run ();

			if (response == ResponseType.Ok)
				outputProjectFolderEntry.Text = System.IO.Path.Combine (fileChooser.CurrentFolderUri, fileChooser.CurrentFolder);
			
			fileChooser.Destroy ();
		}

		void SetActivePhase (ActivePhase phase)
		{
			switch (phase) {
			case ActivePhase.AnalyzeXcodeProject:
				SetControlsSensitivity (true, analysisPhaseControls);
				SetControlsSensitivity (false, generationPhaseControls);
				break;
			case ActivePhase.GenerateCSharpProject:
				SetControlsSensitivity (false, analysisPhaseControls);
				SetControlsSensitivity (true, generationPhaseControls);
				break;
			default:
				break;
			}
		}

		void SetControlsSensitivity (bool sensetive, List<Widget> controls)
		{
			foreach (var control in controls)
				control.Sensitive = sensetive;
		}

		protected void AnalyzeXcodeProject (object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (inputProjectEntry.Text)) {
				ShowAlertDialog ("Xcode project was not selected");
				return;
			}

			if (string.IsNullOrEmpty (outputProjectFolderEntry.Text)) {
				ShowAlertDialog ("Folder of output project can't be empty string");
				return;
			}

			if (OnRunAnalysisButtonPressed != null) {
				targets = OnRunAnalysisButtonPressed (new ConversionPreferences {
					XcodeProjectPath = inputProjectEntry.Text,
					CSharpProjectFolderPath = outputProjectFolderEntry.Text,
					OveriwriteLaunchImages = overwriteLaunchImages.Active,
					OverwriteAppIcons = overwriteAppIcons.Active,
					HeaderSearchPaths = new List<string> (pathToHeaderFilesEntry.Text.Split (new []{ ';' }, StringSplitOptions.RemoveEmptyEntries))
				});

				foreach (var target in targets)
					setProjectTypeCombobox.AppendText (target.Name + ": " + target.ProjectType.ToString ());

				setProjectTypeCombobox.Active = 0;
			}
			
			SetActivePhase (ActivePhase.GenerateCSharpProject);
		}
	}
}

