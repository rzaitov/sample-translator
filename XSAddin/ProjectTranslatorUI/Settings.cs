using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using XcodeProjectParser;

namespace ProjectTranslatorUI
{
	enum ActivePhase {
		None = 0,
		AnalyzeXcodeProject,
		GenerateCSharpProject
	}

	public partial class Settings : Window
	{
		List<XcodeProjectParser.Target> targets;
		List<Widget> analysisPhaseControls;
		List<Widget> generationPhaseControls;
		FileChooserDialog fileChooser;

		public Func<ConversionPreferences, List<XcodeProjectParser.Target>> OnRunAnalysisButtonPressed;
		public Action<List<XcodeProjectParser.Target>> OnRunGenerationButtonPressed;

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
				runButton,
				iosRadio,
				extensionRadio,
				macRadio,
				watchRadio
			};

			iosRadio.Group.Append (extensionRadio);
			iosRadio.Group.Append (macRadio);
			iosRadio.Group.Append (watchRadio);

			SetActivePhase (ActivePhase.AnalyzeXcodeProject);
		}

		protected void SelectInputProject (object sender, EventArgs e)
		{
			var fileFilter = new FileFilter () {
				Name = "*.xcodeproj"
			};

			fileFilter.AddPattern ("*.xcodeproj");

			fileChooser = new FileChooserDialog ("Select Xcode project", this, FileChooserAction.SelectFolder, new object[] {
				Stock.Cancel, ResponseType.Cancel, Stock.Ok, ResponseType.Ok }) {
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
			var alert = new MessageDialog (this,
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
				Stock.Cancel, ResponseType.Cancel, Stock.Ok, ResponseType.Ok }) {
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

			var files = Directory.GetFiles (inputProjectEntry.Text, "*.pbxproj");

			if (files.Length == 0) {
				ShowAlertDialog ("*.pbxproj file not found inside *.xcodeproj folder");
				return;
			}

			if (OnRunAnalysisButtonPressed != null) {
				targets = OnRunAnalysisButtonPressed (new ConversionPreferences {
					XcodeProjectPath = files[0],
					CSharpProjectFolderPath = outputProjectFolderEntry.Text,
					OveriwriteLaunchImages = overwriteLaunchImages.Active,
					OverwriteAppIcons = overwriteAppIcons.Active
				});

				foreach (var target in targets) {
					setProjectTypeCombobox.AppendText (target.Name);
					target.ProjectTypeGuid = SharpProjectType.iOSAppTypeGuid;
				}

				setProjectTypeCombobox.Changed += ProjectSelectionChanged;
				setProjectTypeCombobox.Active = 0;
			}
			
			SetActivePhase (ActivePhase.GenerateCSharpProject);
		}

		void ProjectSelectionChanged (object o, EventArgs args)
		{
			if (targets [setProjectTypeCombobox.Active].ProjectTypeGuid == SharpProjectType.iOSAppTypeGuid) {
				iosRadio.Active = true;
				iosRadio.Toggle ();
			}

			if (targets [setProjectTypeCombobox.Active].ProjectTypeGuid == SharpProjectType.AppExtensionTypeGuid) {
				extensionRadio.Active = true;
				extensionRadio.Toggle ();
			}

			if (targets [setProjectTypeCombobox.Active].ProjectTypeGuid == SharpProjectType.MacTypeGuid) {
				macRadio.Active = true;
				macRadio.Toggle ();
			}

			if (targets [setProjectTypeCombobox.Active].ProjectTypeGuid == SharpProjectType.WatchAppTypeGuid) {
				watchRadio.Active = true;
				watchRadio.Toggle ();
			}
		}

		protected void ProjectTypeChanged (object sender, EventArgs e)
		{
			if (iosRadio.Active)
				targets [setProjectTypeCombobox.Active].ProjectTypeGuid = SharpProjectType.iOSAppTypeGuid;

			if (extensionRadio.Active)
				targets [setProjectTypeCombobox.Active].ProjectTypeGuid = SharpProjectType.AppExtensionTypeGuid;

			if (macRadio.Active)
				targets [setProjectTypeCombobox.Active].ProjectTypeGuid = SharpProjectType.MacTypeGuid;

			if (watchRadio.Active)
				targets [setProjectTypeCombobox.Active].ProjectTypeGuid = SharpProjectType.WatchAppTypeGuid;
		}
	}
}

