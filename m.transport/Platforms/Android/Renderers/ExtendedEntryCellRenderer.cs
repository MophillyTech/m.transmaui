using System;
using Android.Content;
using Android.Views;
using m.transport.Android;
using m.transport;
using View = Android.Views.View;
using Color = Android.Graphics.Color;
using Microsoft.Maui;

using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRenderer(typeof(ExtendedEntryCell), typeof(ExtendedEntryCellRenderer))]
namespace m.transport.Android
{
	public class ExtendedEntryCellRenderer : EntryCellRenderer
	{
		public ExtendedEntryCellRenderer ()
		{
		}

		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var cell = base.GetCellCore(item, convertView, parent, context);
			if (cell != null)
			{
				cell.SetBackgroundColor(Color.Transparent);
			}
			return cell;
		}
	}
}

