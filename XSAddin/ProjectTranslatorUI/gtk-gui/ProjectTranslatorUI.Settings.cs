
// This file has been generated by the GUI designer. Do not modify.
namespace ProjectTranslatorUI
{
	public partial class Settings
	{
		private global::Gtk.Fixed fixed3;
		
		private global::Gtk.Label inputProjectLabel;
		
		private global::Gtk.Label outputProjectLabel;
		
		private global::Gtk.Entry inputProjectEntry;
		
		private global::Gtk.Entry outputProjectFolderEntry;
		
		private global::Gtk.Button selectButton;
		
		private global::Gtk.Button runButton;
		
		private global::Gtk.Button selectOuputFolderButton;
		
		private global::Gtk.CheckButton overwriteAppIcons;
		
		private global::Gtk.CheckButton overwriteLaunchImages;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.HSeparator hseparator2;
		
		private global::Gtk.Label setProjectTypeLabel;
		
		private global::Gtk.ComboBox setProjectTypeCombobox;
		
		private global::Gtk.Button analyzeXcodeProjectButton;
		
		private global::Gtk.RadioButton iosRadio;
		
		private global::Gtk.RadioButton extensionRadio;
		
		private global::Gtk.RadioButton watchRadio;
		
		private global::Gtk.RadioButton macRadio;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget ProjectTranslatorUI.Settings
			this.WidthRequest = 460;
			this.Name = "ProjectTranslatorUI.Settings";
			this.Title = global::Mono.Unix.Catalog.GetString ("Convertor settings");
			this.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-preferences", global::Gtk.IconSize.Menu);
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Resizable = false;
			this.AllowGrow = false;
			this.Gravity = ((global::Gdk.Gravity)(5));
			// Container child ProjectTranslatorUI.Settings.Gtk.Container+ContainerChild
			this.fixed3 = new global::Gtk.Fixed ();
			this.fixed3.WidthRequest = 480;
			this.fixed3.Name = "fixed3";
			this.fixed3.HasWindow = false;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.inputProjectLabel = new global::Gtk.Label ();
			this.inputProjectLabel.Name = "inputProjectLabel";
			this.inputProjectLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Select Xcode project:");
			this.fixed3.Add (this.inputProjectLabel);
			global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.inputProjectLabel]));
			w1.X = 10;
			w1.Y = 10;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.outputProjectLabel = new global::Gtk.Label ();
			this.outputProjectLabel.Name = "outputProjectLabel";
			this.outputProjectLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("C# project folder:");
			this.fixed3.Add (this.outputProjectLabel);
			global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.outputProjectLabel]));
			w2.X = 33;
			w2.Y = 40;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.inputProjectEntry = new global::Gtk.Entry ();
			this.inputProjectEntry.WidthRequest = 200;
			this.inputProjectEntry.CanFocus = true;
			this.inputProjectEntry.Name = "inputProjectEntry";
			this.inputProjectEntry.IsEditable = true;
			this.inputProjectEntry.InvisibleChar = '●';
			this.fixed3.Add (this.inputProjectEntry);
			global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.inputProjectEntry]));
			w3.X = 140;
			w3.Y = 6;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.outputProjectFolderEntry = new global::Gtk.Entry ();
			this.outputProjectFolderEntry.WidthRequest = 200;
			this.outputProjectFolderEntry.CanFocus = true;
			this.outputProjectFolderEntry.Name = "outputProjectFolderEntry";
			this.outputProjectFolderEntry.IsEditable = true;
			this.outputProjectFolderEntry.InvisibleChar = '●';
			this.fixed3.Add (this.outputProjectFolderEntry);
			global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.outputProjectFolderEntry]));
			w4.X = 140;
			w4.Y = 35;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.selectButton = new global::Gtk.Button ();
			this.selectButton.WidthRequest = 100;
			this.selectButton.CanFocus = true;
			this.selectButton.Name = "selectButton";
			this.selectButton.UseUnderline = true;
			this.selectButton.Label = global::Mono.Unix.Catalog.GetString ("Select...");
			this.fixed3.Add (this.selectButton);
			global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.selectButton]));
			w5.X = 350;
			w5.Y = 3;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.runButton = new global::Gtk.Button ();
			this.runButton.WidthRequest = 435;
			this.runButton.CanFocus = true;
			this.runButton.Name = "runButton";
			this.runButton.UseUnderline = true;
			this.runButton.Label = global::Mono.Unix.Catalog.GetString ("Generate C# project");
			this.fixed3.Add (this.runButton);
			global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.runButton]));
			w6.X = 12;
			w6.Y = 260;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.selectOuputFolderButton = new global::Gtk.Button ();
			this.selectOuputFolderButton.WidthRequest = 100;
			this.selectOuputFolderButton.CanFocus = true;
			this.selectOuputFolderButton.Name = "selectOuputFolderButton";
			this.selectOuputFolderButton.UseUnderline = true;
			this.selectOuputFolderButton.Label = global::Mono.Unix.Catalog.GetString ("Select...");
			this.fixed3.Add (this.selectOuputFolderButton);
			global::Gtk.Fixed.FixedChild w7 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.selectOuputFolderButton]));
			w7.X = 350;
			w7.Y = 32;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.overwriteAppIcons = new global::Gtk.CheckButton ();
			this.overwriteAppIcons.CanFocus = true;
			this.overwriteAppIcons.Name = "overwriteAppIcons";
			this.overwriteAppIcons.Label = global::Mono.Unix.Catalog.GetString ("Overwrite app icons with Xamarin assets");
			this.overwriteAppIcons.Active = true;
			this.overwriteAppIcons.DrawIndicator = true;
			this.overwriteAppIcons.UseUnderline = true;
			this.fixed3.Add (this.overwriteAppIcons);
			global::Gtk.Fixed.FixedChild w8 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.overwriteAppIcons]));
			w8.X = 12;
			w8.Y = 80;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.overwriteLaunchImages = new global::Gtk.CheckButton ();
			this.overwriteLaunchImages.CanFocus = true;
			this.overwriteLaunchImages.Name = "overwriteLaunchImages";
			this.overwriteLaunchImages.Label = global::Mono.Unix.Catalog.GetString ("Overwrite launch images with Xamarin assets");
			this.overwriteLaunchImages.Active = true;
			this.overwriteLaunchImages.DrawIndicator = true;
			this.overwriteLaunchImages.UseUnderline = true;
			this.fixed3.Add (this.overwriteLaunchImages);
			global::Gtk.Fixed.FixedChild w9 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.overwriteLaunchImages]));
			w9.X = 12;
			w9.Y = 105;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.WidthRequest = 450;
			this.hseparator1.Name = "hseparator1";
			this.fixed3.Add (this.hseparator1);
			global::Gtk.Fixed.FixedChild w10 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.hseparator1]));
			w10.X = 10;
			w10.Y = 71;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.WidthRequest = 450;
			this.hseparator2.Name = "hseparator2";
			this.fixed3.Add (this.hseparator2);
			global::Gtk.Fixed.FixedChild w11 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.hseparator2]));
			w11.X = 10;
			w11.Y = 161;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.setProjectTypeLabel = new global::Gtk.Label ();
			this.setProjectTypeLabel.Name = "setProjectTypeLabel";
			this.setProjectTypeLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Set project type:");
			this.fixed3.Add (this.setProjectTypeLabel);
			global::Gtk.Fixed.FixedChild w12 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.setProjectTypeLabel]));
			w12.X = 10;
			w12.Y = 170;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.setProjectTypeCombobox = global::Gtk.ComboBox.NewText ();
			this.setProjectTypeCombobox.WidthRequest = 330;
			this.setProjectTypeCombobox.Name = "setProjectTypeCombobox";
			this.fixed3.Add (this.setProjectTypeCombobox);
			global::Gtk.Fixed.FixedChild w13 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.setProjectTypeCombobox]));
			w13.X = 116;
			w13.Y = 165;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.analyzeXcodeProjectButton = new global::Gtk.Button ();
			this.analyzeXcodeProjectButton.WidthRequest = 435;
			this.analyzeXcodeProjectButton.CanFocus = true;
			this.analyzeXcodeProjectButton.Name = "analyzeXcodeProjectButton";
			this.analyzeXcodeProjectButton.UseUnderline = true;
			this.analyzeXcodeProjectButton.Label = global::Mono.Unix.Catalog.GetString ("Analyze Xcode project");
			this.fixed3.Add (this.analyzeXcodeProjectButton);
			global::Gtk.Fixed.FixedChild w14 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.analyzeXcodeProjectButton]));
			w14.X = 12;
			w14.Y = 125;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.iosRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("iOS app"));
			this.iosRadio.CanFocus = true;
			this.iosRadio.Name = "iosRadio";
			this.iosRadio.DrawIndicator = true;
			this.iosRadio.UseUnderline = true;
			this.iosRadio.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.fixed3.Add (this.iosRadio);
			global::Gtk.Fixed.FixedChild w15 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.iosRadio]));
			w15.X = 115;
			w15.Y = 200;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.extensionRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Extension"));
			this.extensionRadio.CanFocus = true;
			this.extensionRadio.Name = "extensionRadio";
			this.extensionRadio.DrawIndicator = true;
			this.extensionRadio.UseUnderline = true;
			this.extensionRadio.Group = this.iosRadio.Group;
			this.fixed3.Add (this.extensionRadio);
			global::Gtk.Fixed.FixedChild w16 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.extensionRadio]));
			w16.X = 115;
			w16.Y = 220;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.watchRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Watch App"));
			this.watchRadio.CanFocus = true;
			this.watchRadio.Name = "watchRadio";
			this.watchRadio.DrawIndicator = true;
			this.watchRadio.UseUnderline = true;
			this.watchRadio.Group = this.iosRadio.Group;
			this.fixed3.Add (this.watchRadio);
			global::Gtk.Fixed.FixedChild w17 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.watchRadio]));
			w17.X = 256;
			w17.Y = 200;
			// Container child fixed3.Gtk.Fixed+FixedChild
			this.macRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Mac"));
			this.macRadio.CanFocus = true;
			this.macRadio.Name = "macRadio";
			this.macRadio.DrawIndicator = true;
			this.macRadio.UseUnderline = true;
			this.macRadio.Group = this.iosRadio.Group;
			this.fixed3.Add (this.macRadio);
			global::Gtk.Fixed.FixedChild w18 = ((global::Gtk.Fixed.FixedChild)(this.fixed3 [this.macRadio]));
			w18.X = 256;
			w18.Y = 220;
			this.Add (this.fixed3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 480;
			this.DefaultHeight = 300;
			this.Show ();
			this.selectButton.Clicked += new global::System.EventHandler (this.SelectInputProject);
			this.runButton.Clicked += new global::System.EventHandler (this.RunUtility);
			this.selectOuputFolderButton.Clicked += new global::System.EventHandler (this.SelectSharpSolutionFolder);
			this.setProjectTypeCombobox.Changed += new global::System.EventHandler (this.ProjectSelectionChanged);
			this.analyzeXcodeProjectButton.Clicked += new global::System.EventHandler (this.AnalyzeXcodeProject);
			this.iosRadio.Toggled += new global::System.EventHandler (this.ProjectTypeChanged);
			this.extensionRadio.Toggled += new global::System.EventHandler (this.ProjectTypeChanged);
			this.watchRadio.Toggled += new global::System.EventHandler (this.ProjectTypeChanged);
			this.macRadio.Toggled += new global::System.EventHandler (this.ProjectTypeChanged);
		}
	}
}