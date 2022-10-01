using System;
using System.ComponentModel;
using System.Diagnostics;
using Autofac;
using m.transport.Data;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using Xamarin;
using m.transport.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using m.transport.Domain;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ExpenseCell : ExtendedViewCell
	{
		public ExpenseCell()
		{
			InitializeComponent();
			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				ExpenseWrapper d = (ExpenseWrapper) this.BindingContext;
			};

		}
			
	}
}
