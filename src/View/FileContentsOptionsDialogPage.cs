﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Microsoft.VisualStudio.ComponentModelHost;
using UnitTestBoilerplate.Services;
using System.ComponentModel.Composition;

namespace UnitTestBoilerplate.View
{
	[Guid("5d7016f4-8aa2-4b43-85f9-1145814471ba")]
	public class FileContentsOptionsDialogPage : UIElementDialogPage
	{
		private FileContentsOptionsDialogPageControl optionsDialogControl;

		protected override UIElement Child
        {
            get { return this.optionsDialogControl ?? (this.optionsDialogControl = new FileContentsOptionsDialogPageControl()); }
        }

		protected override void OnActivate(CancelEventArgs e)
		{
			base.OnActivate(e);

			this.optionsDialogControl.ViewModel.Refresh();
			this.optionsDialogControl.ViewModel.SettingsCoordinator.ReportSettingsPageOpen(this.optionsDialogControl.ViewModel);
		}

		protected override void OnApply(PageApplyEventArgs args)
		{
			if (args.ApplyBehavior == ApplyKind.Apply)
			{
				this.optionsDialogControl.ViewModel.Apply();
			}

			base.OnApply(args);
		}

		protected override void OnClosed(EventArgs e)
		{
			this.optionsDialogControl.ViewModel.SettingsCoordinator.ReportSettingsPageClosed(this.optionsDialogControl.ViewModel);

			base.OnClosed(e);
		}
	}
}
