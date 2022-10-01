using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class CustomNavigationPage : NavigationPage
	{

		public CustomNavigationPage(Page page) : base(page)
		{
			InitializeComponent();
		}
	}
}
