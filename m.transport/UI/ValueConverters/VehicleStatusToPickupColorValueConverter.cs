using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class VehicleStatusToPickupColorValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string vehicleStatus = value.ToString();
			object color;
			switch (vehicleStatus)
			{
				case "Loading":
				case "Delivering":
					Application.Current.Resources.TryGetValue("RedBackground", out color);
					return (Color)color;
				case "Loaded":
				case "Delivered":
					Application.Current.Resources.TryGetValue("GreenBackground", out color);
					return (Color)color;
				default:
					Application.Current.Resources.TryGetValue("WhiteBackground", out color);
					return (Color)color;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
