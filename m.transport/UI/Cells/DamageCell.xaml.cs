using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using m.transport.Models;
using m.transport.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DamageCell : ViewCell
	{
		public DamageCell()
		{
			InitializeComponent();

			var click = new TapGestureRecognizer();
			click.Tapped +=(s,e)=>Delete();
			Trash.GestureRecognizers.Add(click);

			click = new TapGestureRecognizer();
			click.Tapped +=(s,e)=>ToCamera();
			CameraButton.GestureRecognizers.Add(click);

			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				//Device.OnPlatform(iOS: () => {
				//	DamageViewModel d = (DamageViewModel) BindingContext;
				//	if(d == null)
				//		return;
				//	RowLayout.Padding = new Thickness (20, 0, 0, 0);
				//	if(!d.IsDeletable)
				//		Height = 50;
				//	else
				//		Height = 90;

				//}, Android: () => {
				//	CameraButton.MinimumWidthRequest = 70;
				//	CameraButton.WidthRequest = 70;
				//});
                if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                    CameraButton.MinimumWidthRequest = 70;
                    CameraButton.WidthRequest = 70;
                }
                else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    DamageViewModel d = (DamageViewModel)BindingContext;
                    if (d == null)
                        return;
                    RowLayout.Padding = new Thickness(20, 0, 0, 0);
                    if (!d.IsDeletable)
                        Height = 50;
                    else
                        Height = 90;
                }
            };
		}
			
		public void ToCamera()
		{
			//System.Diagnostics.Debug.WriteLine ("getting inspection ");
			Inspection inspect = (Inspection)Parent.GetContentPage ();
			inspect.NavigateToCamera ((DamageViewModel)BindingContext);
		}

		public void Delete()
		{
			DamageViewModel d = (DamageViewModel)BindingContext;
			d.DeleteCommand.Execute(d);
		}
	
	}
}

