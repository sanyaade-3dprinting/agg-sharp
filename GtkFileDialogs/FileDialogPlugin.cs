﻿using System;
using Gtk;
using MatterHackers.Agg.UI;

namespace MatterHackers.Agg.GtkFileDialogs
{
	public class GtkFileDialogPlugin : FileDialogCreator
	{
		// Resolve not needed on non-Mac platforms
		public override string ResolveFilePath(string path) => path;

		public override bool OpenFileDialog(OpenFileDialogParams openParams, Action<OpenFileDialogParams> callback)
		{
			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = true;

			Gtk.FileChooserDialog fc =
				new Gtk.FileChooserDialog(openParams.Title,
					null,
					FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept);

			fc.SetCurrentFolder(openParams.InitialDirectory);
			fc.SelectMultiple = openParams.MultiSelect;

			Gtk.FileFilter filter = new Gtk.FileFilter();
			filter.Name = openParams.Filter.Split('|')[0];
			string[] extensions = openParams.Filter.Split ('|') [1].Split (';');
			foreach (string e in extensions) {
				filter.AddPattern(e.ToLower());
				filter.AddPattern(e.ToUpper());
			}
			fc.AddFilter(filter);

			Gtk.Application.Init();

			if (fc.Run() == (int)ResponseType.Accept)
			{
				openParams.FileNames = fc.Filenames;
				openParams.FileName = fc.Filename;
				UiThread.RunOnIdle((state) =>
					{
						OpenFileDialogParams openParamsIn = state as OpenFileDialogParams;
						if (openParamsIn != null)
						{
							callback(openParamsIn);
						}
					}, openParams);
			}
				
			fc.Destroy();
			while (Gtk.Application.EventsPending()) {
				Gtk.Main.Iteration();
			}

			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = false;

			return true;
		}


		public override bool SelectFolderDialog(SelectFolderDialogParams folderParams, Action<SelectFolderDialogParams> callback)
		{
			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = true;

			Gtk.FileChooserDialog fc =
				new Gtk.FileChooserDialog(folderParams.Description,
					null,
					FileChooserAction.SelectFolder,
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept);

			Gtk.Application.Init();

			if (fc.Run() == (int)ResponseType.Accept)
			{
				folderParams.FolderPath = fc.Filename;
				UiThread.RunOnIdle(() =>
				{
					callback(folderParams);
				});
			}

			fc.Destroy();
			while (Gtk.Application.EventsPending()) {
				Gtk.Main.Iteration();
			}

			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = false;

			return true;
		}

		public override bool SaveFileDialog(SaveFileDialogParams saveParams, Action<SaveFileDialogParams> callback)
		{
			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = true;

			Gtk.FileChooserDialog fc = 
				new Gtk.FileChooserDialog(saveParams.Title,
					null,
					FileChooserAction.Save,
					"Cancel", ResponseType.Cancel,
					"Save", ResponseType.Accept);

			Gtk.FileFilter filter = new Gtk.FileFilter();
			filter.Name = saveParams.Filter.Split('|')[0];
			string[] extensions = saveParams.Filter.Split('|')[1].Split(';');
			foreach (string e in extensions) {
				filter.AddPattern(e.ToLower());
			}
			fc.AddFilter(filter);

			//Set default filename and add file extension
			fc.CurrentName = saveParams.FileName + extensions[0].TrimStart('*');
			fc.SetCurrentFolder(saveParams.InitialDirectory);

			Gtk.Application.Init();

			if (fc.Run() == (int)ResponseType.Accept)
			{
				saveParams.FileName = fc.Filename;
				UiThread.RunOnIdle(() =>
				{
					callback(saveParams);
				});
			}

			fc.Destroy();
			while (Gtk.Application.EventsPending()) {
				Gtk.Main.Iteration();
			}

			WidgetForWindowsFormsAbstract.MainWindowsFormsWindow.ShowingSystemDialog = false;

			return true;
		}
	}
}
