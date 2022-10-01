using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class MenuSelectableToColorValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var enabled = value as bool?;
			object color;
			if (enabled == true)
			{
				bool found = Application.Current.Resources.TryGetValue("LightestText", out color);
				if (found)
				{
					return (Color) color;
				}
			}
			Application.Current.Resources.TryGetValue("DisabledText", out color);
			return (Color)color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
			
	}
}

