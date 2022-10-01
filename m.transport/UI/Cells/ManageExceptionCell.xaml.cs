using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using m.transport.Models;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ManageExceptionCell : ViewCell
	{
		private const int magicNum = 1000;
	
		public ManageExceptionCell ()
		{
			InitializeComponent ();
			Height = 150;

			Toggle.SelectedSegmentChanged += delegate
			{
				if (Toggle.SelectedSegment != null)
				{
					//magicNum used to validate and differenciate the default value 0 and selected value "No"
					((ExceptionViewModel)BindingContext).Vehicle.DatsVehicle.ExceptionFlag = Toggle.SelectedSegment.Value + magicNum + 1;
				}
			};
		}

		public static readonly BindableProperty AllowNullProperty =
			BindableProperty.Create<ManageExceptionCell, bool>(
				p => p.AllowNull, false, BindingMode.OneWay, null, AllowNullPropertyChanged);

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		private static void AllowNullPropertyChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			bool allowNull = newvalue;
			Debug.WriteLine("AllowNull changed to " + allowNull);
			var self = (ManageExceptionCell)bindable;
			self.Toggle.AllowNull = allowNull;
		}
	}
}