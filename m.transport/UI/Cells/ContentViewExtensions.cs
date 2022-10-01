using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public static class ContentViewExtensions 
	{
		public static ContentPage GetContentPage(this Element view)
		{
			var page = view as ContentPage;
			if (page != null)
				return page;
			return GetContentPage(view.Parent);
		}
	}
}
