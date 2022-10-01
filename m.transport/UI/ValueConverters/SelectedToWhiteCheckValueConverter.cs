using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class SelectedToWhiteCheckValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var selected = value as bool?;
			if (selected == true)
			{
				return ImageSource.FromFile("checkmark_w_32.png");
			}
			return ImageSource.FromFile("blank-32.png");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
