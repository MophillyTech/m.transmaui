using System;
using System.Collections.Generic;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class RunDetailsCell : ExtendedViewCell
	{
		public RunDetailsCell ()
		{
			InitializeComponent ();

			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				DatsRunStop d = (DatsRunStop) BindingContext;
			};
		}
	}
}

