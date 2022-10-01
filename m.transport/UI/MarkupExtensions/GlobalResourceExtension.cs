using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	[ContentProperty("Key")]
	public class GlobalResourceExtension : IMarkupExtension
	{
		public string Key { get; set; }
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (this.Key == null)
				throw new InvalidOperationException("you must specify a key in {GlobalResource}");
			if (serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			object value;
			bool found = Application.Current.Resources.TryGetValue(Key, out value);
			if (found)
			{
				return value;
			}
			throw new ArgumentNullException(string.Format("Can't find a global resource for key {0}", Key));
		}
	}
}
