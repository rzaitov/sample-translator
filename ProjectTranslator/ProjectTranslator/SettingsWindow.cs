using System;
using Gtk;

namespace ProjectTranslator
{
	public partial class SettingsWindow: Gtk.Window
	{
		public SettingsWindow () : base (Gtk.WindowType.Toplevel)
		{
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			a.RetVal = true;
		}
	}
}

